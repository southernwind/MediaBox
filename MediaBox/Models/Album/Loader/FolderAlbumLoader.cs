using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Selector;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public class FolderAlbumLoader : AlbumLoader {
		public string DirectoryPath {
			get;
			private set;
		}

		/// <summary>
		/// ファイル追加通知
		/// </summary>
		public override IObservable<IEnumerable<IMediaFileModel>> OnAddFile {
			get {
				return this.mediaFileManager
					.OnRegisteredMediaFiles
					.Select(x => x.Where(m => m.FilePath.StartsWith($@"{this.DirectoryPath}")).Where(this.albumSelector.FilterSetter));
			}
		}

		public FolderAlbumLoader(
			MediaBoxDbContext rdb,
			DocumentDb documentDb,
			MediaFactory mediaFactory,
			AlbumSelector albumSelector,
			NotificationManager notificationManager,
			MediaFileManager mediaFileManager) : base(rdb, documentDb, mediaFactory, albumSelector, notificationManager, mediaFileManager) {
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			return mediaFile => mediaFile.DirectoryPath.StartsWith(this.DirectoryPath);
		}

		/// <summary>
		/// 条件設定用アルバムオブジェクト設定
		/// </summary>
		/// <param name="albumObject">アルバムオブジェクト</param>
		public override void SetAlbumObject(IAlbumObject albumObject) {
			if (albumObject is not FolderAlbumObject fao) {
				throw new ArgumentException();
			}
			this.DirectoryPath = fao.FolderPath;
		}
	}
}