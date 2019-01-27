using System;
using System.Windows;

namespace SandBeige.MediaBox.Library.Map {


	public struct Rectangle {
		public Rectangle(Point leftTop, Size size) {
			this.LeftTop = leftTop;
			this.Size = size;
		}
		public Point LeftTop {
			get;
		}
		public Size Size {
			get;
		}

		/// <summary>
		/// 重なり判定
		/// </summary>
		/// <remarks>
		/// 隣り合っている場合は重なっていないものとする
		/// わかりづらいけど、<see cref="IncludedIn(Point)"/>とはこの条件が少し違う
		/// </remarks>
		/// <param name="rect">判定相手</param>
		/// <returns>true: 重なっている false:重なっていない</returns>
		public bool IntersectsWith(Rectangle rect) {
			return
				Math.Abs(this.LeftTop.X + (this.Size.Width / 2) - (rect.LeftTop.X + (rect.Size.Width / 2))) < (this.Size.Width + rect.Size.Width) / 2 &&
				Math.Abs(this.LeftTop.Y + (this.Size.Height / 2) - (rect.LeftTop.Y + (rect.Size.Height / 2))) < (this.Size.Height + rect.Size.Height) / 2;
		}

		/// <summary>
		/// 矩形の中に指定の座標が含まれているか
		/// </summary>
		/// <remarks>
		/// 辺に重なっている場合は含まれているものとする
		/// わかりづらいけど、<see cref="IntersectsWith(Rectangle)"/>とはこの条件が少し違う
		/// </remarks>
		/// <param name="point">座標</param>
		/// <returns>true: 含まれている false:含まれていない</returns>
		public bool IncludedIn(Point point) {
			return
				this.LeftTop.X <= point.X &&
				point.X <= this.LeftTop.X + this.Size.Width &&
				this.LeftTop.Y <= point.Y &&
				point.Y <= this.LeftTop.Y + this.Size.Height;
		}

		/// <summary>
		/// 矩形同士の距離
		/// </summary>
		/// <param name="rect">相手</param>
		/// <returns>距離</returns>
		public double DistanceTo(Rectangle rect) {
			// 基本はピタゴラスの定理
			// √((c-a)^2+(d-b)^2)

			// 矩形1
			var left1 = this.LeftTop.X;
			var top1 = this.LeftTop.Y;
			var right1 = left1 + this.Size.Width;
			var bottom1 = top1 + this.Size.Height;

			// 矩形2
			var left2 = rect.LeftTop.X;
			var top2 = rect.LeftTop.Y;
			var right2 = left2 + rect.Size.Width;
			var bottom2 = top2 + rect.Size.Height;

			// 重なっていれば距離0
			var x = 0d;
			var y = 0d;

			// 1より2が左側にある
			if (left1 > right2) {
				x = left1 - right2;
			}

			// 1より2が右側にある
			if (right1 < left2) {
				x = left2 - right1;
			}

			// 1より2が上側にある
			if (top1 > bottom2) {
				y = top1 - bottom2;
			}

			// 1より2が下側にある
			if (bottom1 < top2) {
				y = bottom1 - top2;
			}

			return Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
		}

		public override string ToString() {
			return $"<[{base.ToString()}] (LeftTop={this.LeftTop}, Size={this.Size})>";
		}
	}
}
