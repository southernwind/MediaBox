using System.IO;
using System.Linq;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	internal class FolderAlbum : Album {
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
			if (!Directory.Exists(directoryPath)) {
				return;
			}
			this.Items.AddRange(
				Directory
					.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Items.All(m => m.FilePath.Value != x))
					.Select(x => this.MediaFactory.Create(x))
					.ToList());
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override void OnAddedItem(MediaFile mediaFile) {
			mediaFile.CreateThumbnail(ThumbnailLocation.Memory);
			mediaFile.LoadExif();
		}
	}
}
