using System;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// 場所、座標クラス
	/// </summary>
	public class GpsLocation : IComparable<GpsLocation>, IComparable {
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

		public int CompareTo(GpsLocation other) {
			var c = this.Latitude.CompareTo(other.Latitude);
			if (c != 0) {
				return c;
			}
			c = this.Longitude.CompareTo(other.Longitude);
			if (c != 0) {
				return c;
			}
			if (this.Altitude is double alt) {
				c = alt.CompareTo(other.Altitude);
				if (c != 0) {
					return c;
				}
			} else if (other.Altitude is double alt2) {
				c = alt2.CompareTo(this.Altitude);
				if (c != 0) {
					return c;
				}
			}

			return 0;
		}

		public int CompareTo(object obj) {
			if (obj is GpsLocation gl) {
				return this.CompareTo(gl);
			} else {
				return -1;
			}
		}

		public static bool operator ==(GpsLocation gl, GpsLocation gl2) {
			if (gl is null && gl2 is null) {
				return true;
			}
			if (gl is null || gl2 is null) {
				return false;
			}
			return gl.CompareTo(gl2) == 0;
		}

		public static bool operator !=(GpsLocation gl, GpsLocation gl2) {
			return !(gl == gl2);
		}

		public static bool operator <(GpsLocation gl, GpsLocation gl2) {
			return gl.CompareTo(gl2) < 0;
		}

		public static bool operator >(GpsLocation gl, GpsLocation gl2) {
			return gl.CompareTo(gl2) > 0;
		}

		public static bool operator <=(GpsLocation gl, GpsLocation gl2) {
			return gl.CompareTo(gl2) <= 0;
		}

		public static bool operator >=(GpsLocation gl, GpsLocation gl2) {
			return gl.CompareTo(gl2) >= 0;
		}

		public override string ToString() {
			return $"{this.Latitude} {this.Longitude} {this.Altitude}";
		}

		public override bool Equals(object obj) {
			if (!(obj is GpsLocation)) {
				return false;
			}

			var loc = (GpsLocation)obj;
			return this.Latitude == loc.Latitude &&
				this.Longitude == loc.Longitude &&
				this.Altitude == loc.Altitude;
		}

		public override int GetHashCode() {
			return this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode() ^ this.Altitude.GetHashCode();
		}
	}
}
