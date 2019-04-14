using System;

using MetadataExtractor;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// 画像ファイルメタデータ取得インターフェイス
	/// </summary>
	public interface IImage : IDisposable {
		/// <summary>
		/// 幅
		/// </summary>
		int Width {
			get;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		int Height {
			get;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		Rational[] Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		Rational[] Longitude {
			get;
		}

		/// <summary>
		/// 高度
		/// </summary>
		Rational Altitude {
			get;
		}

		/// <summary>
		/// 緯度方向(N/S)
		/// </summary>
		string LatitudeRef {
			get;
		}

		/// <summary>
		/// 経度方向(E/W)
		/// </summary>
		string LongitudeRef {
			get;
		}

		/// <summary>
		/// 高度方向(0/1)
		/// </summary>
		byte AltitudeRef {
			get;
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		int? Orientation {
			get;
		}
	}
}
