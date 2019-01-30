namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// 場所、座標クラス
	/// </summary>
	public class GpsLocation {
		/// <summary>
		/// 緯度
		/// </summary>
		public double Latitude {
			get;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public double Longitude {
			get;
		}

		/// <summary>
		/// 高度
		/// </summary>
		public double? Altitude {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="latitude">緯度</param>
		/// <param name="longitude">経度</param>
		/// <param name="altitude">高度</param>
		public GpsLocation(double latitude, double longitude, double? altitude = null) {
			this.Latitude = latitude;
			this.Longitude = longitude;
			this.Altitude = altitude;
		}
	}
}
