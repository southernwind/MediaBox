using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		/// 2点間の距離(左上同士を比較)
		/// </summary>
		/// <param name="rect">相手</param>
		/// <returns>距離</returns>
		public double DistanceTo(Rectangle rect) {
			// ピタゴラスの定理
			// √((c-a)^2+(d-b)^2)
			return Math.Sqrt(Math.Pow(this.LeftTop.X - rect.LeftTop.X, 2) + Math.Pow(this.LeftTop.Y - rect.LeftTop.Y, 2));
		}
	}
}
