using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Windows.Media;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.Resources;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイル
	/// </summary>
	internal class Thumbnail : ModelBase {
		private ImageSource _imageSource;
		private string _fileName;
		private int? _orientation;
		private byte[] _image;
		private bool _hasError;

		public ThumbnailLocation Location {
			get;
			private set;
		}

		public string FullSizeFilePath {
			get;
			set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName {
			get {
				return this._fileName;
			}
			set {
				this._fileName = value;
				this.Location |= ThumbnailLocation.File;
			}
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public string FilePath {
			get {
				if (this.FileName == null) {
					return null;
				}
				return Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, this.FileName);
			}
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		public int? Orientation {
			get {
				return this._orientation;
			}
			set {
				if (this.RaisePropertyChangedIfSet(ref this._orientation, value)) {
					this.UpdateImageSourceIfLoaded();
				}
			}
		}

		/// <summary>
		/// 画像方向などを適用したイメージソース
		/// </summary>
		public ImageSource ImageSource {
			get {
				if (this._imageSource == null) {
					this.UpdateImageSource();
				}
				return this._imageSource;
			}
			private set {
				this.RaisePropertyChangedIfSet(ref this._imageSource, value);
			}
		}

		/// <summary>
		/// このサムネイルでエラーが発生しているか
		/// </summary>
		public bool HasError {
			get {
				return this._hasError;
			}
			private set {
				this.RaisePropertyChangedIfSet(ref this._hasError, value);
			}
		}

		/// <summary>
		/// イメージソースの更新
		/// </summary>
		private void UpdateImageSource() {
			try {
				this.ImageSource = ImageSourceCreator.Create((object)this.FilePath ?? new MemoryStream(this._image, false), this.Orientation);
				this.HasError = false;
			} catch (Exception ex) {
				this.Logging.Log("サムネイルイメージ生成失敗", LogLevel.Warning, ex);
				this.ImageSource = Images.NoImage;
				this.HasError = true;
			}
		}

		/// <summary>
		/// イメージソースが作成済みなら更新する。
		/// </summary>
		private void UpdateImageSourceIfLoaded() {
			if (this._imageSource == null) {
				return;
			}
			this.UpdateImageSource();
		}

		/// <summary>
		/// 設定されているプロパティ情報からサムネイルの作成
		/// </summary>
		/// <param name="location">サムネイルの作成先</param>
		public void CreateThumbnail(ThumbnailLocation location) {
			var needsUpdate = false;
			if (location == ThumbnailLocation.File) {
				// サムネイルファイルの場合
				byte[] image;
				if (this._image != null) {
					// メモリ上に展開されている場合はそっちを使う
					image = this._image;
				} else {
					// なければフルサイズイメージから作る
					using (var fs = File.OpenRead(this.FullSizeFilePath)) {
						image = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value);
					}
					needsUpdate = true;
				}
				using (var crypto = new SHA256CryptoServiceProvider()) {
					this.FileName = $"{string.Join("", crypto.ComputeHash(image).Select(b => $"{b:X2}"))}.jpg";
					if (!File.Exists(this.FilePath)) {
						File.WriteAllBytes(this.FilePath, image);
					};
				}
			} else {
				// インメモリの場合
				if (this.FilePath == null) {
					// インメモリはサムネイルファイルがない場合のみ作成する
					using (var fs = File.OpenRead(this.FullSizeFilePath)) {
						this._image = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value);
					}
					needsUpdate = true;
				}
			}
			this.Location |= location;
			if (needsUpdate) {
				this.UpdateImageSourceIfLoaded();
			}
		}

		/// <summary>
		/// サムネイルファイル再作成
		/// </summary>
		public void RecreateThumbnail() {
			byte[] image;
			using (var fs = File.OpenRead(this.FullSizeFilePath)) {
				image = ThumbnailCreator.Create(fs, this.Settings.GeneralSettings.ThumbnailWidth.Value, this.Settings.GeneralSettings.ThumbnailHeight.Value);
			}

			using (var crypto = new SHA256CryptoServiceProvider()) {
				this.FileName = $"{string.Join("", crypto.ComputeHash(image).Select(b => $"{b:X2}"))}.jpg";
				if (!File.Exists(this.FilePath)) {
					File.WriteAllBytes(this.FilePath, image);
				}
			}
			this.UpdateImageSourceIfLoaded();
		}
	}
}
