using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.Library.Exif;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	internal class ImageFileModel : MediaFileModel {
		private ImageSource _image;
		private CancellationTokenSource _loadImageCancelToken;
		private int? _orientation;
		private Exif _exif;
		public ImageFileModel(string filePath) : base(filePath) {
		}

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
		/// プロパティ
		/// </summary>
		public override IEnumerable<TitleValuePair<string>> Properties {
			get {
				return base.Properties.Concat(this._exif?.ToTitleValuePair() ?? Array.Empty<TitleValuePair<string>>());
			}
		}

		/// <summary>
		/// 画像読み込み
		/// </summary>
		public async Task LoadImageAsync() {
			if (this._loadImageCancelToken != null) {
				return;
			}
			this._loadImageCancelToken = new CancellationTokenSource();
			this.Image =
				await ImageSourceCreator.CreateAsync(
					this.FilePath,
					this.Orientation,
					token: this._loadImageCancelToken.Token);
			this._loadImageCancelToken = null;

		}

		/// <summary>
		/// 読み込んだ画像破棄
		/// </summary>
		public void UnloadImage() {
			this._loadImageCancelToken?.Cancel();
			this.Image = null;
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public override void CreateThumbnail() {
			try {
				using (var fs = File.OpenRead(this.FilePath)) {
#if LOAD_LOG
					this.Logging.Log($"[Thumbnail Create]{this.FileName}");
#endif
					// TODO : あとからOrientationが変化した場合の対応
					var image = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value, this.Orientation);
					if (this.ThumbnailLocation.HasFlag(ThumbnailLocation.Memory)) {
						this.Thumbnail.Binary = image;
					}
					if (this.ThumbnailLocation.HasFlag(ThumbnailLocation.File)) {
						using (var crypto = new SHA256CryptoServiceProvider()) {
							this.Thumbnail.FileName = $"{string.Join("", crypto.ComputeHash(image).Select(b => $"{b:X2}"))}.jpg";
							if (!File.Exists(this.Thumbnail.FilePath)) {
								File.WriteAllBytes(this.Thumbnail.FilePath, image);
							};
						}
					}
				}
				base.CreateThumbnail();
			} catch (ArgumentException ex) {
				// TODO : ログ出力だけでいいのか、検討
				this.Logging.Log($"{this.FilePath}画像が不正なため、サムネイルの作成に失敗しました。", LogLevel.Warning, ex);
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
		/// プロパティの内容をデータベースへ登録
		/// </summary>
		/// <returns>登録したレコード</returns>
		public override MediaFile RegisterToDataBase() {
			using (var transaction = this.DataBase.Database.BeginTransaction()) {
				var mf = base.RegisterToDataBase();
				mf.ImageFile = new ImageFile {
					Orientation = this.Orientation
				};
				lock (this.DataBase) {
					this.DataBase.SaveChanges();
				}
				transaction.Commit();
				return mf;
			}
		}

		/// <summary>
		/// ファイル情報読み込み
		/// </summary>
		public override void GetFileInfo() {
#if LOAD_LOG
			this.Logging.Log($"[Exif Load]{this.FileName}");
#endif
			this._exif = new Exif(this.FilePath);
			if (new object[] { this._exif.GPSLatitude, this._exif.GPSLongitude, this._exif.GPSLatitudeRef, this._exif.GPSLongitudeRef }.All(l => l != null)) {
				this.Latitude = (this._exif.GPSLatitude[0] + (this._exif.GPSLatitude[1] / 60) + (this._exif.GPSLatitude[2] / 3600)) * (this._exif.GPSLongitudeRef == "S" ? -1 : 1);
				this.Longitude = (this._exif.GPSLongitude[0] + (this._exif.GPSLongitude[1] / 60) + (this._exif.GPSLongitude[2] / 3600)) * (this._exif.GPSLongitudeRef == "W" ? -1 : 1);
			}
			this.Orientation = this._exif.Orientation;
			base.GetFileInfo();
		}

		/// <summary>
		/// 表示用画像
		/// </summary>
		public ImageSource Image {
			get {
				return this._image;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._image, value);
			}
		}
	}
}
