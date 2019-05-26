﻿using System;
using System.IO;
using System.Linq;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;

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
				var path = Thumbnail.GetThumbnailRelativeFilePath(this.FilePath);
				var ffmpeg = new Library.Video.FFmpeg(this.Settings.PathSettings.FFmpegDirectoryPath.Value);
				ffmpeg.CreateThumbnail(
					this.FilePath,
					Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, path),
					this.Settings.GeneralSettings.ThumbnailWidth.Value,
					this.Settings.GeneralSettings.ThumbnailHeight.Value);
				this.RelativeThumbnailFilePath = path;
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
		/// プロパティの内容でデータベースレコードを更新
		/// </summary>
		/// <param name="targetRecord">更新対象レコード</param>
		public override void UpdateDataBaseRecord(MediaFile targetRecord) {
			try {
				var ffmpeg = new Library.Video.FFmpeg(this.Settings.PathSettings.FFmpegDirectoryPath.Value);
				var meta = ffmpeg.ExtractMetadata(this.FilePath);

				if (!this.LoadedFromDataBase) {
					this.Duration = meta.Duration;
					this.Rotation = meta.Rotation;
					this.Location = meta.Location;
					if (meta.Rotation % 180 == 0) {
						this.Resolution = new ComparableSize(meta.Width ?? double.NaN, meta.Height ?? double.NaN);
					} else {
						this.Resolution = new ComparableSize(meta.Height ?? double.NaN, meta.Width ?? double.NaN);
					}
				}

				base.UpdateDataBaseRecord(targetRecord);
				targetRecord.VideoFile ??= new VideoFile();

				targetRecord.VideoFile.Duration = this.Duration;
				targetRecord.VideoFile.Rotation = this.Rotation;
			} catch (Exception ex) {
				this.Logging.Log("メタデータ取得失敗", LogLevel.Warning, ex);
				base.UpdateDataBaseRecord(targetRecord);
				this.IsInvalid = true;
			}
		}
	}
}
