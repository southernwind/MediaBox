using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Media;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.Resources;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイル
	/// </summary>
	internal class Thumbnail : ModelBase, IThumbnail {
		private ImageSource _imageSource;
		private bool _hasError;

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName {
			get;
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
		/// <remarks>
		/// エラーフラグ
		/// </remarks>
		public bool HasError {
			get {
				return this._hasError;
			}
			private set {
				this.RaisePropertyChangedIfSet(ref this._hasError, value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="fileName">サムネイルファイル名</param>
		public Thumbnail(string fileName) {
			this.FileName = fileName;
		}

		/// <summary>
		/// イメージソースの更新
		/// </summary>
		/// <remarks>
		/// ファイルパスがあればファイルパスから生成、なければメモリ上のサムネイル画像イメージから生成する。
		/// 両方ない場合はエラーフラグが立つ。
		/// </remarks>
		private void UpdateImageSource() {
			try {
				if (this.FilePath != null) {
					this.ImageSource = ImageSourceCreator.Create(this.FilePath);
					this.HasError = false;
				}
			} catch (Exception ex) {
				this.Logging.Log("サムネイルイメージ生成失敗", LogLevel.Warning, ex);
				this.ImageSource = Images.NoImage;
				this.HasError = true;
			}
		}

		/// <summary>
		/// サムネイルファイル名取得
		/// </summary>
		/// <param name="filePath">生成元ファイルパス</param>
		/// <returns>サムネイルファイル名</returns>
		public static string GetThumbnailFileName(string filePath) {
			using (var crypto = new SHA256CryptoServiceProvider()) {
				return $"{string.Join("", crypto.ComputeHash(Encoding.UTF8.GetBytes(filePath)).Select(b => $"{b:X2}"))}".Insert(2, @"\");
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.FileName}>";
		}
	}
}
