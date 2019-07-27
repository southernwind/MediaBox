using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイル監視
	/// </summary>
	internal class MediaFileManager : ModelBase {
		private readonly object _registerItemsLockObject = new object();
		/// <summary>
		/// メディアファイル登録通知用Subject
		/// </summary>
		private readonly Subject<IEnumerable<IMediaFileModel>> _onRegisteredMediaFilesSubject = new Subject<IEnumerable<IMediaFileModel>>();

		/// <summary>
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnRegisteredMediaFiles {
			get {
				return this._onRegisteredMediaFilesSubject.AsObservable();
			}
		}

		/// <summary>
		/// 読み込み状態
		/// </summary>
		public ReadOnlyReactiveCollection<MediaFileDirectoryMonitoring> LoadStates {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MediaFileManager() {
			this.LoadStates = this.Settings
				.ScanSettings
				.ScanDirectories
				.ToReadOnlyReactiveCollection(sd => {
					var dm = new MediaFileDirectoryMonitoring(sd);
					dm.NewFileNotification.Subscribe(this.RegisterItems);
					dm.DeleteFileNotification.Subscribe(x => {
						foreach (var item in x) {
							item.Exists = false;
						}
					});
					return dm;
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFiles">登録ファイル</param>
		private void RegisterItems(IEnumerable<IMediaFileModel> mediaFiles) {
			lock (this._registerItemsLockObject) {
				var files = mediaFiles.Select(x => x.FilePath);
				MediaFile[] mfs;
				lock (this.DataBase) {
					mfs = this.DataBase
						.MediaFiles
						.Include(x => x.AlbumMediaFiles)
						.Include(x => x.ImageFile)
						.Include(x => x.VideoFile)
						.Include(x => x.Jpeg)
						.Include(x => x.Png)
						.Include(x => x.Bmp)
						.Include(x => x.Gif)
						.Include(x => x.Position)
						.Where(x => files.Contains(x.FilePath))
						.ToArray();
				}

				// データ登録キューへ追加
				var addList = new List<(IMediaFileModel model, MediaFile record)>();
				var updateList = new List<(IMediaFileModel model, MediaFile record)>();
				var joined =
					mediaFiles
						.GroupJoin(
							mfs,
							model => model.FilePath,
							record => record.FilePath,
							(x, y) => (model: x, record: y.FirstOrDefault())
						);
				foreach (var mf in joined) {
					if (mf.record != null) {
						updateList.Add(mf);
					} else {
						addList.Add((mf.model, mf.model.CreateDataBaseRecord()));
					}
				}
				lock (this.DataBase) {
					using var transaction = this.DataBase.Database.BeginTransaction(IsolationLevel.ReadUncommitted);
					this.DataBase.MediaFiles.AddRange(addList.Select(t => t.record));

					foreach (var (model, record) in updateList) {
						model.UpdateDataBaseRecord(record);
					}

					// 必要な座標情報の事前登録
					var prs = updateList
						.Union(addList)
						.Select(x => x.record)
						.Where(x => x.Latitude != null && x.Longitude != null)
						.Select(x => (Latitude: (double)x.Latitude, Longitude: (double)x.Longitude))
						.Except(this.DataBase.Positions.ToList().Select(x => (x.Latitude, x.Longitude)))
						.Select(x => new Position() { Latitude = x.Latitude, Longitude = x.Longitude })
						.ToList();
					this.DataBase.Positions.AddRange(prs);

					this.DataBase.SaveChanges();
					foreach (var (model, record) in addList) {
						model.MediaFileId = record.MediaFileId;
					}
					transaction.Commit();
				}

				this._onRegisteredMediaFilesSubject.OnNext(addList.Select(t => t.model));
			}
		}
	}
}
