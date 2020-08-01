using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Selector;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public abstract class AlbumLoader : ModelBase {
		protected readonly MediaBoxDbContext rdb;
		private readonly DocumentDb _documentDb;
		private readonly MediaFactory _mediaFactory;
		protected readonly AlbumSelector albumSelector;
		private readonly NotificationManager _notificationManager;
		protected readonly MediaFileManager mediaFileManager;

		/// <summary>
		/// ファイル削除通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnDeleteFile {
			get {
				return this.mediaFileManager.OnDeletedMediaFiles;
			}
		}

		/// <summary>
		/// ファイル追加通知
		/// </summary>
		public virtual IObservable<IEnumerable<IMediaFileModel>> OnAddFile {
			get;
		}

		/// <summary>
		/// アルバム定義更新通知
		/// </summary>
		public virtual IObservable<Unit> OnAlbumDefinitionUpdated {
			get {
				return Observable.Empty<Unit>();
			}
		}

		public AlbumLoader(MediaBoxDbContext rdb, DocumentDb documentDb, MediaFactory mediaFactory, AlbumSelector albumSelector, NotificationManager notificationManager, MediaFileManager mediaFileManager) {
			this.rdb = rdb;
			this._documentDb = documentDb;
			this._mediaFactory = mediaFactory;
			this._notificationManager = notificationManager;

			this.albumSelector = albumSelector;
			this.mediaFileManager = mediaFileManager;
		}

		/// <summary>
		/// フィルタリング前件数取得
		/// </summary>
		public int GetBeforeFilteringCount() {
			lock (this.rdb) {
				return this._documentDb.GetMediaFilesCollection().Query().Where(this.WherePredicate()).Count();
			}
		}

		/// <summary>
		/// メディアファイルリスト読み込み
		/// </summary>
		public async Task<IEnumerable<IMediaFileModel>> LoadMediaFiles(TaskActionState state) {
			return await Task.Run(() => {
				try {
					using (this.DisposeLock.DisposableEnterReadLock()) {
						if (this.DisposeState != DisposeState.NotDisposed) {
							return null;
						}

						if (state.CancellationToken.IsCancellationRequested) {
							return null;
						}

						MediaFile[] items;
						lock (this.rdb) {
							items = this._documentDb
								.GetMediaFilesCollection()
								.Query()
								.Where(this.WherePredicate())
								.Include(mf => mf.Position)
								.Where(this.albumSelector.FilterSetter)
								.ToArray();
						}

						var mediaFiles = new IMediaFileModel[items.Length];
						foreach (var (item, index) in items.Select((x, i) => (x, i))) {
							if (state.CancellationToken.IsCancellationRequested) {
								return null;
							}

							var m = this._mediaFactory.Create(item.FilePath);
							if (!m.FileInfoLoaded) {
								m.LoadFromDataBase(item);
								m.UpdateFileInfo();
							}

							mediaFiles[index] = m;
						}

						return this.albumSelector.SortSetter.SetSortConditions(mediaFiles);
					}

				} catch (Exception e) {
					this._notificationManager.Notify(new Error(null, e.ToString()));
					return new IMediaFileModel[] { };
				}
			});
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected abstract Expression<Func<MediaFile, bool>> WherePredicate();

		/// <summary>
		/// 条件設定用アルバムオブジェクト設定
		/// </summary>
		/// <param name="albumObject">アルバムオブジェクト</param>
		public abstract void SetAlbumObject(IAlbumObject albumObject);
	}
}
