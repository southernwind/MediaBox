using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Map;

namespace SandBeige.MediaBox.Library.Tests.Map {
	[TestFixture]
	internal class RectangleTest {
		[TestCase(true, 10, 10, 10, 10)]
		[TestCase(false, 20, 20, 40, 40)]
		[TestCase(false, 20, 10, 40, 40)]
		[TestCase(false, 10, 20, 40, 40)]
		[TestCase(true, 19.9, 19.9, 4, 4)]
		[TestCase(false, 5, 5, 5, 15)]
		[TestCase(true, 5, 5, 5.1, 5.1)]
		public void IntersectsWith(bool result, double x, double y, double w, double h) {
			var rect = new Rectangle(new Point(10, 10), new Size(10, 10));

			var rect2 = new Rectangle(new Point(x, y), new Size(w, h));
			Assert.AreEqual(result, rect.IntersectsWith(rect2));
			Assert.AreEqual(result, rect2.IntersectsWith(rect));
		}

		[TestCase(true, 10, 10)]
		[TestCase(false, 9.9, 10)]
		[TestCase(true, 15, 15)]
		[TestCase(true, 10, 15)]
		[TestCase(true, 15, 10)]
		[TestCase(false, 15, 9.9)]
		[TestCase(true, 20, 20)]
		[TestCase(false, 20.1, 20)]
		[TestCase(false, 20, 20.1)]
		public void IncludedIn(bool result, double x, double y) {
			var rect = new Rectangle(new Point(10, 10), new Size(10, 10));
			var point = rect.IncludedIn(new Point(x, y));
			Assert.AreEqual(result, point);
		}

		[TestCase(0, 10, 10, 10, 10)]
		[TestCase(0, 10, 10, 40, 40)]
		[TestCase(90.1, 100.1, 10, 40, 40)]
		[TestCase(90.1, 10, 100.1, 40, 40)]
		[TestCase(5, 13, 14, 4, 4)]
		[TestCase(1.41421356, 11, 11, 4, 4)]
		[TestCase(43.6615391, 50.3, 26.8, 4, 4)]
		[TestCase(5, 6, 7, 4, 4)]
		public void DistanceTo(double result, double x, double y, double w, double h) {
			var rect = new Rectangle(new Point(10, 10), new Size(10, 10));

			var rect2 = new Rectangle(new Point(x, y), new Size(w, h));
			Assert.AreEqual(rect.DistanceTo(rect2), result, 0.000001);
			Assert.AreEqual(rect2.DistanceTo(rect), result, 0.000001);
		}
	}
}
