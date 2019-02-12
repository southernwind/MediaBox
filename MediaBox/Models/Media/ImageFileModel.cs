using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

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
		private IEnumerable<TitleValuePair<IEnumerable<TitleValuePair<string>>>> _properties;


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
				return base.Properties.Concat(this._properties?.SelectMany(x => x.Value) ?? Array.Empty<TitleValuePair<string>>());
			}
		}

		/// <summary>
		/// 表示用画像 ない場合はサムネイルを表示用とする
		/// </summary>
		/// <remarks>
		/// <see cref="LoadImageAsync"/>と<see cref="UnloadImage"/>で読み込んだり破棄したりする。
		/// 結構なメモリを使用するので破棄しないとたいへんなことになる。
		/// </remarks>
		public ImageSource Image {
			get {
				return this._image ?? this.Thumbnail.ImageSource;
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
		/// 画像読み込み
		/// </summary>
		public async Task LoadImageAsync() {
			if (this._loadImageCancelToken != null) {
				return;
			}
#if LOAD_LOG
			this.Logging.Log($"[load full image] {this}");
#endif
			this._loadImageCancelToken = new CancellationTokenSource();
			try {
				this.Image =
					await ImageSourceCreator.CreateAsync(
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
					var image = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value, this.Orientation);
					if (this.ThumbnailLocation.HasFlag(Composition.Enum.ThumbnailLocation.Memory)) {
						this.Thumbnail.Binary = image;
					}
					if (this.ThumbnailLocation.HasFlag(Composition.Enum.ThumbnailLocation.File)) {
						using (var crypto = new SHA256CryptoServiceProvider()) {
							var name = $"{string.Join("", crypto.ComputeHash(image).Select(b => $"{b:X2}"))}.jpg";
							var path = Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, name);
							if (!File.Exists(path)) {
								File.WriteAllBytes(path, image);
							};
							this.Thumbnail.FileName = name;
						}
					}
				}
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
			try {
				using (var meta = ImageMetadataFactory.Create(File.OpenRead(this.FilePath))) {
					this._properties = meta.Properties;
					if (!this.LoadedFromDataBase) {
						if (new object[] { meta.Latitude, meta.Longitude, meta.LatitudeRef, meta.LongitudeRef }.All(l => l != null)) {
							this.Location = new GpsLocation(
								(meta.Latitude[0].ToDouble() + (meta.Latitude[1].ToDouble() / 60) + (meta.Latitude[2].ToDouble() / 3600)) * (meta.LatitudeRef == "S" ? -1 : 1),
								(meta.Longitude[0].ToDouble() + (meta.Longitude[1].ToDouble() / 60) + (meta.Longitude[2].ToDouble() / 3600)) * (meta.LongitudeRef == "W" ? -1 : 1)
							);
						}
						this.Orientation = meta.Orientation;

						// ExifのOrientationを加味
						if (this.Orientation >= 5) {
							this.Resolution = new ComparableSize(meta.Height, meta.Width);
						} else {
							this.Resolution = new ComparableSize(meta.Width, meta.Height);
						}
					}
				}
				base.GetFileInfo();
			} catch (Exception ex) {
				this.Logging.Log("ファイル情報取得失敗", LogLevel.Warning, ex);
				this.IsInvalid = true;
			}
		}
	}
}
