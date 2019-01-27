using System.IO;
using System.Linq;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class FolderAlbum : AlbumModel {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderAlbum(string path) {
			this.Title.Value = path;
			this.MonitoringDirectories.Add(path);
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		protected override void LoadFileInDirectory(string directoryPath) {
			this.ThumbnailLocation = ThumbnailLocation.Memory;
			this.Items.AddRange(
				Directory
					.EnumerateFiles(directoryPath)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Items.All(m => m.FilePath != x))
					.Select(x => this.MediaFactory.Create(x, this.ThumbnailLocation))
					.ToList());
		}

		/// <summary>
		/// ファイルシステムイベント
		/// </summary>
		/// <param name="e">作成・更新・改名・削除などのイベント情報</param>
		protected override void OnFileSystemEvent(FileSystemEventArgs e) {
			if (!e.FullPath.IsTargetExtension()) {
				return;
			}

			switch (e.ChangeType) {
				case WatcherChangeTypes.Created:
					this.Items.Add(this.MediaFactory.Create(e.FullPath, this.ThumbnailLocation));
					break;
				case WatcherChangeTypes.Deleted:
					this.Items.Remove(this.Items.Single(i => i.FilePath == e.FullPath));
					break;
			}
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override void OnAddedItem(MediaFileModel mediaFile) {
			mediaFile.GetFileInfoIfNotLoaded();
			mediaFile.CreateThumbnailIfNotExists();
		}
	}
}
