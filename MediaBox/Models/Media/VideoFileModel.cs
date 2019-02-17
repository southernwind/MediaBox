using System;
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

				var ffmpeg = new Library.Video.FFmpeg(this.Settings.PathSettings.FFmpegDirectoryPath.Value);
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

		/// <summary>
		/// ファイル情報取得
		/// </summary>
		public override void GetFileInfo() {
			try {
				var ffmpeg = new Library.Video.FFmpeg(this.Settings.PathSettings.FFmpegDirectoryPath.Value);
				var meta = ffmpeg.ExtractMetadata(this.FilePath);
				this.Metadata =
					new[] {
						new TitleValuePair<Attributes<string>>("Formats", meta.Formats)
					}.Concat(
						meta
							.Streams
							.Select(
								(x, i) =>
									new TitleValuePair<Attributes<string>>($"Stream[{i}]", x)
					)).ToAttributes();

				if (!this.LoadedFromDataBase) {
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
		/// データベース登録
		/// </summary>
		/// <returns>生成したレコード</returns>
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
