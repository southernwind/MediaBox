using System;
using System.Linq;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	internal class VideoFileModel : MediaFileModel {
		private int? _rotation;
		private double? _duration;

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

		/// <summary>
		/// プロパティ
		/// </summary>
		public override Attributes<string> Properties {
			get {
				return
					base.Properties.Concat(
						new Attributes<string> {
							{ "動画の長さ",$"{this.Duration}秒" },
							{ "回転",$"{this.Rotation}°" }
						}).ToAttributes();
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath"></param>
		public VideoFileModel(string filePath) : base(filePath) {
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public override void CreateThumbnail() {
			try {
				var thumb = Get.Instance<IThumbnail>(Media.Thumbnail.GetThumbnailFileName(this.FilePath));
				var ffmpeg = new Library.Video.FFmpeg(this.Settings.PathSettings.FFmpegDirectoryPath.Value);
				ffmpeg.CreateThumbnail(
					this.FilePath,
					thumb.FilePath,
					this.Settings.GeneralSettings.ThumbnailWidth.Value,
					this.Settings.GeneralSettings.ThumbnailHeight.Value);
				this.Thumbnail = thumb;
				base.CreateThumbnail();
			} catch (Exception ex) {
				this.Logging.Log("サムネイル作成失敗", LogLevel.Warning, ex);
				this.IsInvalid = true;
			}
		}

		/// <summary>
		/// データベース読み込み
		/// </summary>
		/// <param name="record">読み込み元レコード情報</param>
		public override void LoadFromDataBase(MediaFile record) {
			base.LoadFromDataBase(record);
			this.Duration = record.VideoFile.Duration;
			this.Rotation = record.VideoFile.Rotation;
		}

		/// <summary>
		/// プロパティの内容からデータベースレコードを作成
		/// </summary>
		/// <returns>レコード</returns>
		public override MediaFile CreateDataBaseRecord() {
			var ffmpeg = new Library.Video.FFmpeg(this.Settings.PathSettings.FFmpegDirectoryPath.Value);
			var meta = ffmpeg.ExtractMetadata(this.FilePath);

			if (!this.LoadedFromDataBase) {
				this.Duration = meta.Duration;
				this.Rotation = meta.Rotation;
				this.Location = meta.Location;
				this.Resolution = new ComparableSize(meta.Width ?? double.NaN, meta.Height ?? double.NaN);
			}

			var mf = base.CreateDataBaseRecord();
			mf.VideoFile = new VideoFile {
				Duration = this.Duration,
				Rotation = this.Rotation
			};
			return mf;
		}
	}
}
