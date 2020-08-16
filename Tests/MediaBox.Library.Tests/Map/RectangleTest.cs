using System.Windows;

using FluentAssertions;

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
			rect.IntersectsWith(rect2).Should().Be(result);
			rect2.IntersectsWith(rect).Should().Be(result);
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
			rect.IncludedIn(new Point(x, y)).Should().Be(result);
		}

		[TestCase(0, 10, 10, 10, 10)]
		[TestCase(0, 20, 20, 1, 1)]
		[TestCase(80.1, 100.1, 10, 40, 40)]
		[TestCase(80.1, 10, 100.1, 40, 40)]
		[TestCase(5, 23, 24, 4, 4)]
		[TestCase(1.41421356, 21, 21, 4, 4)]
		[TestCase(43.6615391, 60.3, 36.8, 4, 4)]
		[TestCase(5, 1, 5, 5, 2)]
		[TestCase(6, 12, 3, 12, 1)]
		[TestCase(6, 20, 2, 7, 2)]
		[TestCase(10, 28, 1, 6, 3)]
		[TestCase(3, 23, 8, 8, 2)]
		[TestCase(7, 27, 14, 42, 5)]
		[TestCase(2, 5, 18, 3, 7)]
		[TestCase(3, 20, 23, 7, 2)]
		[TestCase(5, 23, 24, 2, 5)]
		[TestCase(7.0710678, 3, 25, 2, 13)]
		[TestCase(10, 13, 30, 4, 15)]
		[TestCase(50, -25, -45, 5, 15)]
		public void DistanceTo(double result, double x, double y, double w, double h) {
			var rect = new Rectangle(new Point(10, 10), new Size(10, 10)); // (10,10) (10,20) (20,20) (20,10)の正方形

			var rect2 = new Rectangle(new Point(x, y), new Size(w, h));
			Assert.AreEqual(result, rect.DistanceTo(rect2), 0.000001);
			Assert.AreEqual(result, rect2.DistanceTo(rect), 0.000001);
		}
	}
}
