using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Library.Video;

namespace SandBeige.MediaBox.Models.Media {
	internal class VideoFileModel : MediaFileModel {
		private int? _rotation;
		private double? _duration;

		/// <summary>
		/// プロパティ
		/// </summary>
		public override IEnumerable<TitleValuePair<string>> Properties {
			get {
				return
					base.Properties.Concat(
					new Dictionary<string, string> {
						{ "動画の長さ",$"{this.Duration}秒" },
						{ "回転",$"{this.Rotation}°" }
					}.ToTitleValuePair());
			}
		}

		/// <summary>
		/// 動画の長さ
		/// </summary>
		public double? Duration {
			get {
				return this._duration;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._duration, value);
			}
		}

		/// <summary>
		/// 回転
		/// </summary>
		public int? Rotation {
			get {
				return this._rotation;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._rotation, value);
			}
		}

		public VideoFileModel(string filePath) : base(filePath) {
		}

		public override void CreateThumbnail() {
			try {

				var ffmpeg = new FFmpeg(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Externals\ffmpeg"));
				this.Thumbnail.FileName = ffmpeg.CreateThumbnail(
					this.FilePath,
					this.Settings.PathSettings.ThumbnailDirectoryPath.Value,
					this.Settings.GeneralSettings.ThumbnailWidth.Value,
					this.Settings.GeneralSettings.ThumbnailHeight.Value);
				base.CreateThumbnail();
			} catch (Exception ex) {
				this.Logging.Log("サムネイル作成失敗", LogLevel.Warning, ex);
				this.IsInvalid = true;
			}
		}

		public override void GetFileInfo() {
			try {
				if (!this.LoadedFromDataBase) {
					var ffmpeg = new FFmpeg(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Externals\ffmpeg"));
					var meta = ffmpeg.ExtractMetadata(this.FilePath);
					this.Duration = meta.Duration;
					this.Rotation = meta.Rotation;
					this.Location = meta.Location;
					this.Resolution = new ComparableSize(meta.Width ?? double.NaN, meta.Height ?? double.NaN);
				}
				base.GetFileInfo();
			} catch (Exception ex) {
				this.Logging.Log("ファイル情報取得失敗", LogLevel.Warning, ex);
				this.IsInvalid = true;
			}
		}

		public override void LoadFromDataBase(MediaFile record) {
			base.LoadFromDataBase(record);
			this.Duration = record.VideoFile.Duration;
			this.Rotation = record.VideoFile.Rotation;
		}

		public override MediaFile RegisterToDataBase() {
			using (var transaction = this.DataBase.Database.BeginTransaction()) {
				var mf = base.RegisterToDataBase();
				mf.VideoFile = new VideoFile {
					Duration = this.Duration,
					Rotation = this.Rotation
				};
				lock (this.DataBase) {
					this.DataBase.SaveChanges();
				}
				transaction.Commit();
				return mf;
			}
		}
	}
}
