using System;

namespace SandBeige.MediaBox.Composition.Objects {
	public struct ComparableSize : IComparable<ComparableSize>, IComparable {
		/// <summary>
		/// 幅
		/// </summary>
		public double Width {
			get;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public double Height {
			get;
		}

		/// <summary>
		/// 面積
		/// </summary>
		public double Area {
			get;
		}

		public ComparableSize(double width, double height) : this() {
			this.Width = width;
			this.Height = height;
			if (double.IsNaN(this.Width) || double.IsNaN(this.Height)) {
				this.Area = double.NaN;
			} else {
				this.Area = width * height;
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
	}
}
