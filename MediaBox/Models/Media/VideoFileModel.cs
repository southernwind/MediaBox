using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Collection;

namespace SandBeige.MediaBox.Models.Media {
	internal class VideoFileModel : MediaFileModel {
		public override IEnumerable<TitleValuePair> Properties {
			get {
				return Array.Empty<TitleValuePair>();
			}
		}

		public VideoFileModel(string filePath) : base(filePath) {
		}

		public override void CreateThumbnail(ThumbnailLocation location) {
			var thumbnailFileName = "";
			using (var crypto = new SHA256CryptoServiceProvider()) {
				thumbnailFileName = $"{string.Join("", crypto.ComputeHash(Encoding.UTF8.GetBytes(this.FilePath)).Select(b => $"{b:X2}").ToArray())}.jpg";
			}
			var thumbnailFilePath = Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, thumbnailFileName);
			var thumbSize = $"{this.Settings.GeneralSettings.ThumbnailWidth.Value}x{this.Settings.GeneralSettings.ThumbnailHeight.Value}";

			var process = Process.Start(new ProcessStartInfo {
				FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Externals\ffmpeg\ffmpeg.exe"),
				Arguments = $"-ss 0 -i \"{this.FilePath}\" -vframes 1 -f image2 -s {thumbSize} \"{thumbnailFilePath}\" -y",
				CreateNoWindow = true,
				UseShellExecute = false
			});
			process.WaitForExit();
			this.Thumbnail.FileName = thumbnailFileName;
		}

		public override void GetFileInfo() {
			var process = Process.Start(new ProcessStartInfo {
				FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Externals\ffmpeg\ffprobe.exe"),
				Arguments = $"{this.FilePath} -hide_banner -show_entries stream -show_entries format",
				CreateNoWindow = true,
				RedirectStandardError = true,
				RedirectStandardOutput = true,
				UseShellExecute = false
			});
			var info = process.StandardOutput.ReadToEnd();
			base.GetFileInfo();
		}

		public override void LoadFromDataBase(MediaFile record) {
			base.LoadFromDataBase(record);
		}

		public override MediaFile RegisterToDataBase() {
			var mf = new MediaFile {
				FilePath = this.FilePath,
				ThumbnailFileName = this.Thumbnail.FileName,
				Latitude = this.Latitude,
				Longitude = this.Longitude,
				Date = this.Date,
				FileSize = this.FileSize,
				Rate = this.Rate
			};
			lock (this.DataBase) {
				this.DataBase.MediaFiles.Add(mf);
				this.DataBase.SaveChanges();
			}
			this.MediaFileId = mf.MediaFileId;
			return mf;
		}

	}
}
