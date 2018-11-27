using System.IO;
using System.Linq;
using System.Reactive.Linq;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album {
	class RegisteredAlbum :Album {

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <returns>this</returns>
		public Album Initialize() {
			this.Title.Value = "すべて";
			this.RegisteredFileLoad();
			this.MonitoringDirectories = this.Settings.PathSettings.MonitoringDirectories;
			this.BeginMonitoring();
			return this;
		}

		/// <summary>
		/// データベースからメディアファイルの読み込み
		/// </summary>
		private void RegisteredFileLoad() {
			this.Items.AddRangeOnScheduler(
				this.DataBase.MediaFiles.AsEnumerable().Select(x => {
					var m = Get.Instance<MediaFile>().Initialize(ThumbnailLocation.File, Path.Combine(x.DirectoryPath, x.FileName));
					m.Thumbnail.Value = Get.Instance<Thumbnail>().Initialize(x.ThumbnailFileName);
					m.Latitude.Value = x.Latitude;
					m.Longitude.Value = x.Longitude;
					return m;
				})
			);
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
					.Where(x => this.DataBase.MediaFiles.All(m => Path.GetFileName(x) != m.FileName || Path.GetDirectoryName(x) != m.DirectoryPath))
					.Select(x => Get.Instance<MediaFile>().Initialize(ThumbnailLocation.File, x))
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
			var dbmf = new DataBase.Tables.MediaFile() {
				DirectoryPath = Path.GetDirectoryName(mediaFile.FilePath.Value),
				FileName = mediaFile.FileName.Value,
				ThumbnailFileName = mediaFile.Thumbnail.Value.FileName,
				Latitude = mediaFile.Latitude.Value,
				Longitude = mediaFile.Longitude.Value
			};
			this.DataBase.MediaFiles.Add(dbmf);
			this.DataBase.SaveChanges();
			mediaFile.MediaFileId = dbmf.MediaFileId;
		}
	}
}
