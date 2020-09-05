using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public class FolderAlbumLoader : AlbumLoader {
		public string? DirectoryPath {
			get;
			private set;
		}

		/// <summary>
		/// ファイル追加通知
		/// </summary>
		public override IObservable<IEnumerable<IMediaFileModel>> OnAddFile {
			get {
				if (this.FilterSetter == null) {
					throw new InvalidOperationException();
				}
				return this.mediaFileManager
					.OnRegisteredMediaFiles
					.Select(x => x.Where(m => m.FilePath.StartsWith($@"{this.DirectoryPath}")).Where(this.FilterSetter));
			}
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public override string Title {
			get {
				if (this.DirectoryPath == null) {
					throw new InvalidOperationException();
				}
				return this.DirectoryPath;
			}
			set {
			}
		}

		public FolderAlbumLoader(
			IMediaBoxDbContext rdb,
			IDocumentDb documentDb,
			IMediaFactory mediaFactory,
			INotificationManager notificationManager,
			IMediaFileManager mediaFileManager) : base(rdb, documentDb, mediaFactory, notificationManager, mediaFileManager) {
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			if (this.DirectoryPath == null) {
				throw new InvalidOperationException();
			}
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