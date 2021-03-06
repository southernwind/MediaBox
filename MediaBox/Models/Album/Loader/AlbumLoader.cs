using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Notification;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public abstract class AlbumLoader : ModelBase, IAlbumLoader {
		protected readonly IMediaBoxDbContext Rdb;
		private readonly IMediaFactory _mediaFactory;
		private readonly INotificationManager _notificationManager;
		protected readonly IMediaFileManager MediaFileManager;
		protected IFilterDescriptionManager? FilterSetter;
		protected ISortDescriptionManager? SortSetter;

		/// <summary>
		/// ファイル削除通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnDeleteFile {
			get {
				return this.MediaFileManager.OnDeletedMediaFiles;
			}
		}

		/// <summary>
		/// ファイル追加通知
		/// </summary>
		public virtual IObservable<IEnumerable<IMediaFileModel>> OnAddFile {
			get {
				return Observable.Never<IEnumerable<IMediaFileModel>>();
			}
		}

		/// <summary>
		/// アルバム定義更新通知
		/// </summary>
		public virtual IObservable<Unit> OnAlbumDefinitionUpdated {
			get {
				return Observable.Empty<Unit>();
			}
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public abstract string Title {
			get;
			set;
		}

		protected AlbumLoader(IMediaBoxDbContext rdb, IMediaFactory mediaFactory, INotificationManager notificationManager, IMediaFileManager mediaFileManager) {
			this.Rdb = rdb;
			this._mediaFactory = mediaFactory;
			this._notificationManager = notificationManager;
			this.MediaFileManager = mediaFileManager;
		}

		/// <summary>
		/// フィルタリング前件数取得
		/// </summary>
		public int GetBeforeFilteringCount() {
			lock (this.Rdb) {
				return this.Rdb.MediaFiles.Where(this.WherePredicate()).Count();
			}
		}

		/// <summary>
		/// メディアファイルリスト読み込み
		/// </summary>
		public async Task<IEnumerable<IMediaFileModel>?> LoadMediaFiles(TaskActionState state) {
			if (this.FilterSetter == null || this.SortSetter == null) {
				throw new InvalidOperationException();
			}

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
						lock (this.Rdb) {
							items = this.Rdb
								.MediaFiles
								.Where(this.WherePredicate())
								.Include(mf => mf.MediaFileTags)
								.ThenInclude(mft => mft.Tag)
								.Include(mf => mf.ImageFile)
								.Include(mf => mf.VideoFile)
								.Include(mf => mf.Position)
								.Where(this.FilterSetter)
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

						return this.SortSetter.SetSortConditions(mediaFiles);
					}

				} catch (Exception e) {
					this._notificationManager.Notify(new Error(null, e.ToString()));
					return Array.Empty<IMediaFileModel>();
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

		/// <summary>
		/// フィルターソート設定
		/// </summary>
		/// <param name="filterSetter">フィルター</param>
		/// <param name="sortSetter">ソート</param>
		public void SetFilterAndSort(IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter) {
			this.FilterSetter = filterSetter;
			this.SortSetter = sortSetter;
		}
	}
}
