using System.IO;
using System.Linq;

using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using MetadataExtractor.Formats.Jpeg;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// Jpegメタデータ取得クラス
	/// </summary>
	public class Jpeg : ImageBase {
		/// <summary>
		/// 幅
		/// </summary>
		public override int Width {
			get;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public override int Height {
			get;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public override Rational[] Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public override Rational[] Longitude {
			get;
		}

		/// <summary>
		/// 緯度方向(N/S)
		/// </summary>
		public override string LatitudeRef {
			get;
		}

		/// <summary>
		/// 経度方向(E/W)
		/// </summary>
		public override string LongitudeRef {
			get;
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		public override int? Orientation {
			get;
		}

		/// <summary>
		/// メタデータの値と名前のペアのリストをを持つタグディレクトリのリスト
		/// </summary>
		public override Attributes<Attributes<string>> Properties {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="stream">画像ファイルストリーム</param>
		internal Jpeg(Stream stream) : base(stream) {
			var reader = JpegMetadataReader.ReadMetadata(stream);
			this.Properties = reader.ToProperties();
			var d = reader.First(x => x is JpegDirectory);
			var gps = reader.FirstOrDefault(x => x is GpsDirectory);
			var ifd0 = reader.FirstOrDefault(x => x is ExifDirectoryBase);
			this.Width = d.GetUInt16(JpegDirectory.TagImageWidth);
			this.Height = d.GetUInt16(JpegDirectory.TagImageHeight);

			if (ifd0 != null && ifd0.TryGetUInt16(ExifDirectoryBase.TagOrientation, out var orientation)) {
				this.Orientation = orientation;
			}

			if (gps != null) {
				this.Latitude = gps.GetRationalArray(GpsDirectory.TagLatitude);
				this.Longitude = gps.GetRationalArray(GpsDirectory.TagLongitude);
				this.LatitudeRef = gps.GetString(GpsDirectory.TagLatitudeRef);
				this.LongitudeRef = gps.GetString(GpsDirectory.TagLatitudeRef);
			}
		}
	}
}
