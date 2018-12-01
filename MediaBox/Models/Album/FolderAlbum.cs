using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	class FolderAlbum : Album {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FolderAlbum(string path) {
			this.Title.Value = path;
			this.MonitoringDirectories.Add(path);
			this.BeginMonitoring();
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		protected override void Load(string directoryPath) {
			if (!Directory.Exists(directoryPath)) {
				return;
			}
			this.Queue.AddRange(
				Directory
					.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Queue.All(m => m.FilePath.Value != x))
					.Select(x => Get.Instance<MediaFile>(x))
					.ToList());
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override void AddItem(MediaFile mediaFile) {
			mediaFile.CreateThumbnail(ThumbnailLocation.Memory);
			mediaFile.LoadExif();
			this.Items.Add(mediaFile);
		}
	}
}
