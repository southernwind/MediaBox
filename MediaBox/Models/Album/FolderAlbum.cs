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
			this.Items.AddRange(
				Directory
					.EnumerateFiles(directoryPath)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Items.All(m => m.FilePath != x))
					.Select(x => this.MediaFactory.Create(x))
					.ToList());
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override void OnAddedItem(MediaFileModel mediaFile) {
			mediaFile.GetFileInfo();
			mediaFile.CreateThumbnailIfNotExists(ThumbnailLocation.Memory);
		}
	}
}
