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
		/// サムネイルファイルパス
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> ThumbnailFilePath {
			get;
			private set;
		}

		/// <summary>
		/// サムネイルファイル名
		/// </summary>
		public ReactivePropertySlim<string> ThumbnailFileName {
			get;
		} = new ReactivePropertySlim<string>();

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
		public MediaFile Initialize(string filePath) {
			this.FilePath.Value = filePath;
			this.FileName = this.FilePath.Select(x => Path.GetFileName(x)).ToReadOnlyReactiveProperty();
			this.ThumbnailFilePath = this.ThumbnailFileName.Where(x => x != null).Select(x => Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, x)).ToReadOnlyReactivePropertySlim();
			return this;
		}

		/// <summary>
		/// サムネイル作成
		/// </summary>
		public void CreateThumbnail() {
			using (var crypto = new SHA256CryptoServiceProvider()) {
				var file = File.ReadAllBytes(this.FilePath.Value);

				var thumbnail = ThumbnailCreator.Create(file, 200, 200).ToArray();
				var thumbFileName = $"{string.Join("", crypto.ComputeHash(thumbnail).Select(b => $"{b:X2}"))}.jpg";
				var thumbFilePath = Path.Combine(this.Settings.PathSettings.ThumbnailDirectoryPath.Value, thumbFileName);
				if (!File.Exists(thumbFilePath)) {
					File.WriteAllBytes(thumbFilePath, thumbnail);
				}
				this.ThumbnailFileName.Value = thumbFileName;
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
}
