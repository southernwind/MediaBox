using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;

using Livet;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	/// <summary>
	/// フォルダアルバム
	/// </summary>
	/// <remarks>
	/// </remarks>
	internal class FolderAlbum : AlbumModel {
		public string DirectoryPath {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderAlbum(string path) : base(new ObservableSynchronizedCollection<IMediaFileModel>()) {
			this.Title.Value = path;
			this.DirectoryPath = path;
			this.Load();

			var mfm = Get.Instance<MediaFileManager>();
			mfm
				.OnRegisteredMediaFiles
				.Subscribe(x => {
					lock (this.Items.SyncRoot) {
						this.Items.AddRange(x.Where(m => m.FilePath.StartsWith($@"{this.DirectoryPath}")));
					}
				});

			mfm
				.OnFileSystemEvent
				.Where(x => x.ChangeType == WatcherChangeTypes.Deleted)
				.Subscribe(x => {
					lock (this.Items.SyncRoot) {
						// TODO : 作成後すぐに削除を行うと登録前に削除通知がくるかもしれないので考慮が必要
						this.Items.RemoveRange(this.Items.Where(i => i.FilePath.StartsWith($@"{x.FullPath}\") || i.FilePath == x.FullPath));
					}
				});
		}

		/// <summary>
		/// アルバム読み込み条件絞り込み
		/// </summary>
		/// <returns>絞り込み関数</returns>
		protected override Expression<Func<MediaFile, bool>> WherePredicate() {
			return mediaFile => mediaFile.DirectoryPath.StartsWith(this.DirectoryPath);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Title.Value}>";
		}

	}
}
