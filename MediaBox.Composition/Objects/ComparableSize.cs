using System;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// 比較可能なサイズ構造体
	/// </summary>
	public struct ComparableSize : IComparable<ComparableSize>, IComparable {
		private double _width;
		private double _height;
		/// <summary>
		/// 幅
		/// </summary>
		public double Width {
			get {
				return this._width;
			}
			set {
				this._width = value;
				this.UpdateArea();
			}
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public double Height {
			get {
				return this._height;
			}
			set {
				this._height = value;
				this.UpdateArea();
			}
		}

		/// <summary>
		/// 面積
		/// </summary>
		public double Area {
			get;
			private set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		public ComparableSize(double width, double height) : this() {
			this.Width = width;
			this.Height = height;
		}

		/// <summary>
		/// 面積更新
		/// </summary>
		private void UpdateArea() {
			if (double.IsNaN(this.Width) || double.IsNaN(this.Height)) {
				this.Area = double.NaN;
			} else {
				this.Area = this.Width * this.Height;
			}
		}

		public int CompareTo(ComparableSize other) {
			if (this.Area > other.Area) {
				return 1;
			} else if (this.Area < other.Area) {
				return -1;
			} else {
				return 0;
			}
		}

		public int CompareTo(object obj) {
			if (obj is ComparableSize cs) {
				return this.CompareTo(cs);
			} else {
				return -1;
			}
		}

		public static bool operator ==(ComparableSize cs, ComparableSize cs2) {
			return cs.CompareTo(cs2) == 0;
		}

		public static bool operator !=(ComparableSize cs, ComparableSize cs2) {
			return cs.CompareTo(cs2) != 0;
		}

		public static bool operator <(ComparableSize cs, ComparableSize cs2) {
			return cs.CompareTo(cs2) < 0;
		}

		public static bool operator >(ComparableSize cs, ComparableSize cs2) {
			return cs.CompareTo(cs2) > 0;
		}

		public static bool operator <=(ComparableSize cs, ComparableSize cs2) {
			return cs.CompareTo(cs2) <= 0;
		}

		public static bool operator >=(ComparableSize cs, ComparableSize cs2) {
			return cs.CompareTo(cs2) >= 0;
		}

		public override string ToString() {
			return $"{this.Width}x{this.Height}";
		}

		public override bool Equals(object obj) {
			if (!(obj is ComparableSize)) {
				return false;
			}

			var size = (ComparableSize)obj;
			return this.Width == size.Width &&
				   this.Height == size.Height;
		}

		public override int GetHashCode() {
			return this.Width.GetHashCode() ^ this.Height.GetHashCode();
		}
	}
}
