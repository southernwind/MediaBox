﻿using System;
using System.Collections.Generic;
using System.IO;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Library.Video;

namespace SandBeige.MediaBox.Models.Media {
	internal class VideoFileModel : MediaFileModel {
		private int? _rotation;
		private double? _duration;
		public override IEnumerable<TitleValuePair> Properties {
			get {
				return new Dictionary<string, string> {
					{ "動画の長さ",$"{this.Duration}秒" },
					{ "回転",$"{this.Rotation}°" }
				}.ToTitleValuePair();
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
				this.RaisePropertyChangedIfSet(ref this._duration, value, nameof(this.Properties));
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
				this.RaisePropertyChangedIfSet(ref this._rotation, value, nameof(this.Properties));
			}
		}

		public VideoFileModel(string filePath) : base(filePath) {
		}

		public override void CreateThumbnail(ThumbnailLocation location) {
			var ffmpeg = new FFmpeg(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Externals\ffmpeg"));
			ffmpeg.CreateThumbnail(
				this.FilePath,
				this.Settings.PathSettings.ThumbnailDirectoryPath.Value,
				this.Settings.GeneralSettings.ThumbnailWidth.Value,
				this.Settings.GeneralSettings.ThumbnailHeight.Value);
		}

		public override void GetFileInfo() {
			var ffmpeg = new FFmpeg(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Externals\ffmpeg"));
			var meta = ffmpeg.ExtractMetadata(this.FilePath);
			this.Duration = meta.Duration;
			this.Rotation = meta.Rotation;
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
