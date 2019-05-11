using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Media;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.Library.Image;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// 画像ファイルモデル
	/// </summary>
	/// <remarks>
	/// 画像専用プロパティの定義と取得、登録を行う。
	/// </remarks>
	internal class ImageFileModel : MediaFileModel {
		private ImageSource _image;
		private CancellationTokenSource _loadImageCancelToken;
		private int? _orientation;

		/// <summary>
		/// 画像の回転
		/// </summary>
		public int? Orientation {
			get {
				return this._orientation;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._orientation, value);
			}
		}

		/// <summary>
		/// 表示用画像 ない場合はサムネイルを表示用とする
		/// </summary>
		public ImageSource Image {
			get {
				return this._image ?? this.Thumbnail?.ImageSource;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._image, value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath">画像ファイルパス</param>
		public ImageFileModel(string filePath) : base(filePath) {
		}

		/// <summary>
		/// フルイメージロード済みでなければ読み込む
		/// </summary>
		public void LoadImageIfNotLoaded() {
			if (this._image != null) {
				return;
			}
			this.LoadImage();
		}

		/// <summary>
		/// 画像読み込み
		/// </summary>
		public void LoadImage() {
			if (this._loadImageCancelToken != null) {
				return;
			}
#if LOAD_LOG
			this.Logging.Log($"[load full image] {this}");
#endif
			this._loadImageCancelToken = new CancellationTokenSource();
			try {
				this.Image =
					ImageSourceCreator.Create(
						this.FilePath,
						this.Orientation,
						token: this._loadImageCancelToken.Token);
			} catch (Exception) {
				this.IsInvalid = true;
			}
			this._loadImageCancelToken = null;

		}

		/// <summary>
		/// 読み込んだ画像破棄
		/// </summary>
		public void UnloadImage() {
#if LOAD_LOG
			this.Logging.Log($"[unload full image] {this}");
#endif
			this._loadImageCancelToken?.Cancel();
			this.Image = null;
			GC.Collect(2, GCCollectionMode.Forced);
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public override void CreateThumbnail() {
			try {
				var thumb = Get.Instance<IThumbnail>(Media.Thumbnail.GetThumbnailFileName(this.FilePath));
				using (var fs = File.OpenRead(this.FilePath)) {
#if LOAD_LOG
					this.Logging.Log($"[Thumbnail Create]{this.FileName}");
#endif
					var image = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value, this.Orientation);
					File.WriteAllBytes(thumb.FilePath, image);
				}

				this.Thumbnail = thumb;
				base.CreateThumbnail();
			} catch (Exception ex) {
				this.Logging.Log("サムネイル作成失敗", LogLevel.Warning, ex);
				this.IsInvalid = true;
			}
		}

		/// <summary>
		/// データベースからプロパティ読み込み
		/// </summary>
		/// <param name="record">データベースレコード</param>
		public override void LoadFromDataBase(MediaFile record) {
			base.LoadFromDataBase(record);
			this.Orientation = record.ImageFile.Orientation;
		}

		/// <summary>
		/// プロパティの内容からデータベースレコードを作成
		/// </summary>
		/// <param name="targetRecord">更新対象レコード</param>
		public override void UpdateDataBaseRecord(MediaFile targetRecord) {
			try {
				using var meta = ImageMetadataFactory.Create(File.OpenRead(this.FilePath));
				if (new object[] { meta.Latitude, meta.Longitude, meta.LatitudeRef, meta.LongitudeRef }.All(l => l != null)) {
					this.Location = new GpsLocation(
						(meta.Latitude[0].ToDouble() + (meta.Latitude[1].ToDouble() / 60) + (meta.Latitude[2].ToDouble() / 3600)) * (meta.LatitudeRef == "S" ? -1 : 1),
						(meta.Longitude[0].ToDouble() + (meta.Longitude[1].ToDouble() / 60) + (meta.Longitude[2].ToDouble() / 3600)) * (meta.LongitudeRef == "W" ? -1 : 1),
						meta.Altitude.ToDouble() * (meta.AltitudeRef == 1 ? -1 : 1)
					);
				} else {
					this.Location = null;
				}
				this.Orientation = meta.Orientation;

				// ExifのOrientationを加味
				if (this.Orientation >= 5) {
					this.Resolution = new ComparableSize(meta.Height, meta.Width);
				} else {
					this.Resolution = new ComparableSize(meta.Width, meta.Height);
				}

				base.UpdateDataBaseRecord(targetRecord);

				if (meta is Library.Image.Formats.Jpeg jpeg) {
					targetRecord.Jpeg ??= new DataBase.Tables.Metadata.Jpeg();
					jpeg.UpdateRowdata(targetRecord.Jpeg);
					// TODO : ファイルのハッシュに変更する
					targetRecord.Hash = "111";
				} else if (meta is Library.Image.Formats.Png png) {
					targetRecord.Png ??= new DataBase.Tables.Metadata.Png();
					png.UpdateRowdata(targetRecord.Png);
					targetRecord.Hash = "111";
				} else if (meta is Library.Image.Formats.Bmp bmp) {
					targetRecord.Bmp ??= new DataBase.Tables.Metadata.Bmp();
					bmp.UpdateRowdata(targetRecord.Bmp);
					targetRecord.Hash = "111";
				} else if (meta is Library.Image.Formats.Gif gif) {
					targetRecord.Gif ??= new DataBase.Tables.Metadata.Gif();
					gif.UpdateRowdata(targetRecord.Gif);
					targetRecord.Hash = "111";
				}
				targetRecord.ImageFile ??= new ImageFile();
				targetRecord.ImageFile.Orientation = this.Orientation;
			} catch (Exception) {
				this.IsInvalid = true;
				base.UpdateDataBaseRecord(targetRecord);
				targetRecord.ImageFile ??= new ImageFile();
			}
		}
	}
}
