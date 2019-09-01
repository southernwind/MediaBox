using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.IO;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

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
		/// メディアファイル削除通知用Subject
		/// </summary>
		private readonly Subject<IEnumerable<IMediaFileModel>> _onDeletedMediaFilesSubject = new Subject<IEnumerable<IMediaFileModel>>();

		/// <summary>
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnRegisteredMediaFiles {
			get {
				return this._onRegisteredMediaFilesSubject.AsObservable();
			}
		}

		/// <summary>
		/// メディアファイル登録通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnDeletedMediaFiles {
			get {
				return this._onDeletedMediaFilesSubject.AsObservable();
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
					dm.NewFileNotification.Subscribe(this.RegisterItemsCore);
					dm.DeleteFileNotification.Subscribe(x => {
						foreach (var item in x) {
							item.Exists = false;
						}
					});
					return dm;
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// データベースからファイルを削除
		/// </summary>
		/// <param name="mediaFiles">削除するファイル</param>
		public void DeleteItems(IEnumerable<IMediaFileModel> mediaFiles) {
			lock (this._registerItemsLockObject) {
				var files = mediaFiles.Select(x => x.FilePath);
				lock (this.DataBase) {
					using var transaction = this.DataBase.Database.BeginTransaction();
					var removeRows = this.DataBase.MediaFiles.Where(x => files.Contains(x.FilePath)).ToArray();
					this.DataBase.MediaFiles.RemoveRange(removeRows);
					this.DataBase.SaveChanges();
					transaction.Commit();
				}
				this._onDeletedMediaFilesSubject.OnNext(mediaFiles);
			}
		}


		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="directoryPath">登録するファイルを含んでいるフォルダパス</param>
		public void RegisterItems(string directoryPath) {
			Get.Instance<PriorityTaskQueue>().AddTask(
				new TaskAction($"データベース登録[{directoryPath}]",
				async state => await Task.Run(() => {
					(string path, long size)[] files;
					lock (this.DataBase) {
						files = this.DataBase
							.MediaFiles
							.Select(x => new { x.FilePath, x.FileSize })
							.AsEnumerable()
							.Select(x => (x.FilePath, x.FileSize))
							.ToArray();
					}

					var newItems = DirectoryEx
						.EnumerateFiles(directoryPath, true)
						.Where(x => x.IsTargetExtension())
						.Where(x => !files.Any(f => x == f.path && new FileInfo(x).Length == f.size))
						.ToArray();

					state.ProgressMax.Value = newItems.Length;
					foreach (var item in newItems.Select(x => this.MediaFactory.Create(x)).Buffer(100)) {
						if (state.CancellationToken.IsCancellationRequested) {
							return;
						}
						this.RegisterItemsCore(item);
						state.ProgressValue.Value += item.Count;
					}
				}),
				Priority.RegisterMediaFiles,
				new CancellationTokenSource()));
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFiles">登録ファイル</param>
		public void RegisterItems(IEnumerable<string> mediaFilePaths) {
			Get.Instance<PriorityTaskQueue>().AddTask(
				new TaskAction("データベース登録",
					async state => await Task.Run(() => {
						(string path, long size)[] files;
						lock (this.DataBase) {
							files = this.DataBase
								.MediaFiles
								.Select(x => new { x.FilePath, x.FileSize })
								.AsEnumerable()
								.Select(x => (x.FilePath, x.FileSize))
								.ToArray();
						}

						var newMediaFiles = mediaFilePaths.Select(this.MediaFactory.Create)
							.Where(x => x.FilePath.IsTargetExtension())
							.Where(x => !files.Any(f => x.FilePath == f.path && new FileInfo(x.FilePath).Length == f.size))
							.ToArray();

						state.ProgressMax.Value = newMediaFiles.Count();
						foreach (var item in newMediaFiles.Buffer(100)) {
							if (state.CancellationToken.IsCancellationRequested) {
								return;
							}
							this.RegisterItemsCore(item);
							state.ProgressValue.Value += item.Count;
						}
					}),
					Priority.RegisterMediaFiles,
					new CancellationTokenSource()));
		}

		/// <summary>
		/// データベースへファイルを登録
		/// </summary>
		/// <param name="mediaFiles">登録ファイル</param>
		private void RegisterItemsCore(IEnumerable<IMediaFileModel> mediaFiles) {
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
						this.NotificationManager.Notify(new Information(mf.model.ThumbnailFilePath, $"ファイルが登録されました。{Environment.NewLine} [{mf.model.FileName}]"));
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
