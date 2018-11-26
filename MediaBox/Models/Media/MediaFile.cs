using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Cryptography;
using Reactive.Bindings;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファイルクラス
	/// </summary>
	internal class MediaFile : ModelBase {
		/// <summary>
		/// サムネイル保存場所
		/// </summary>
		private ThumbnailLocation _thumbnailLocation;

		public long? MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public ReadOnlyReactiveProperty<string> FileName {
			get;
			private set;
		}

		/// <summary>
		/// ファイルパス
		/// </summary>
		public ReactivePropertySlim<string> FilePath {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// サムネイル
		/// </summary>
		public ReactivePropertySlim<Thumbnail> Thumbnail {
			get;
		} = new ReactivePropertySlim<Thumbnail>();

		/// <summary>
		/// 緯度
		/// </summary>
		public ReactivePropertySlim<double?> Latitude {
			get;
		} = new ReactivePropertySlim<double?>();

		/// <summary>
		/// 経度
		/// </summary>
		public ReactivePropertySlim<double?> Longitude {
			get;
		} = new ReactivePropertySlim<double?>();

		/// <summary>
		/// Exif情報
		/// </summary>
		public ReactiveProperty<Exif> Exif {
			get;
		} = new ReactiveProperty<Exif>();

		/// <summary>
		/// 初期処理
		/// </summary>
		/// <param name="filePath">ファイルパス</param>
		/// <returns><see cref="this"/></returns>
		public MediaFile Initialize(ThumbnailLocation thumbnailLocation,string filePath) {
			this._thumbnailLocation = thumbnailLocation;
			this.FilePath.Value = filePath;
			this.FileName = this.FilePath.Select(x => Path.GetFileName(x)).ToReadOnlyReactiveProperty();
			return this;
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public void CreateThumbnail() {
			using (var fs = File.OpenRead(this.FilePath.Value)) {
				var thumbnailByteArray = ThumbnailCreator.Create(fs, 200, 200);
				if (this._thumbnailLocation == ThumbnailLocation.File) {
					using (var crypto = new SHA256CryptoServiceProvider()) {
						var thumbnail = Get.Instance<Thumbnail>().Initialize($"{string.Join("", crypto.ComputeHash(thumbnailByteArray).Select(b => $"{b:X2}"))}.jpg");
						if (!File.Exists(thumbnail.FilePath)) {
							File.WriteAllBytes(thumbnail.FilePath, thumbnailByteArray);
						}

						this.Thumbnail.Value = thumbnail;
					}
				} else {
					this.Thumbnail.Value = Get.Instance<Thumbnail>().Initialize(thumbnailByteArray);
				}
			}
		}

		public void LoadExifIfNotLoaded() {
			if (this.Exif.Value == null) {
				this.LoadExif();
			}
		}

		public void LoadExif() {
			var exif = new Exif(this.FilePath.Value);
			this.Exif.Value = exif;
			if (new object[] { exif.GPSLatitude, exif.GPSLongitude, exif.GPSLatitudeRef, exif.GPSLongitudeRef }.All(l => l != null)) {
				this.Latitude.Value = (exif.GPSLatitude[0] + (exif.GPSLatitude[1] / 60) + exif.GPSLatitude[2] / 3600) * (exif.GPSLongitudeRef == "S" ? -1 : 1);
				this.Longitude.Value = (exif.GPSLongitude[0] + (exif.GPSLongitude[1] / 60) + exif.GPSLongitude[2] / 3600) * (exif.GPSLongitudeRef == "W" ? -1 : 1);
			}
		}
	}

	/// <summary>
	/// サムネイル生成場所
	/// </summary>
	public enum ThumbnailLocation {
		/// <summary>
		/// ファイル
		/// </summary>
		File,
		/// <summary>
		/// メモリ上
		/// </summary>
		Memory
	}
}
