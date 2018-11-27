using System.IO;
using System.Linq;
using System.Reactive.Linq;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	class FolderAlbum : Album {
		/// <summary>
		/// 初期処理
		/// </summary>
		/// <returns>this</returns>
		public virtual Album Initialize(string path) {
			this.Title.Value = path;
			this.MonitoringDirectories = new ReactiveCollection<IMonitoringDirectory>();
			var md = Get.Instance<IMonitoringDirectory>();
			md.DirectoryPath.Value = path;
			md.Monitoring.Value = true;
			this.MonitoringDirectories.Add(md);
			this.BeginMonitoring();
			return this;
		}

		/// <summary>
		/// ディレクトリパスからメディアファイルの読み込み
		/// </summary>
		/// <param name="directoryPath">ディレクトリパス</param>
		protected override void Load(string directoryPath) {
			if (!Directory.Exists(directoryPath)) {
				return;
			}
			this.Queue.AddRangeOnScheduler(
				Directory
					.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
					.Where(x => x.IsTargetExtension())
					.Where(x => this.Queue.All(m => m.FilePath.Value != x))
					.Select(x => Get.Instance<MediaFile>().Initialize(ThumbnailLocation.Memory,x))
					.ToList());
		}

		/// <summary>
		/// メディアファイル追加
		/// </summary>
		/// <param name="mediaFile"></param>
		protected override void AddItem(MediaFile mediaFile) {
			mediaFile.CreateThumbnail();
			mediaFile.LoadExif();
			this.Items.Add(mediaFile);
		}
	}
}
