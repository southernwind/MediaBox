using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Creator;
using SandBeige.MediaBox.Library.EventAsObservable;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Library.Tests.Creator {
	[TestFixture]
	public class ImageSourceCreatorTest {

		private static string _testDataDir;

		[SetUp]
		public void SetUp() {
			_testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
		}

		[TestCase(Rotation.Rotate0, false, null)]
		[TestCase(Rotation.Rotate0, false, 0)]
		[TestCase(Rotation.Rotate0, false, 1)]
		[TestCase(Rotation.Rotate0, true, 2)]
		[TestCase(Rotation.Rotate180, false, 3)]
		[TestCase(Rotation.Rotate180, true, 4)]
		[TestCase(Rotation.Rotate270, true, 5)]
		[TestCase(Rotation.Rotate90, false, 6)]
		[TestCase(Rotation.Rotate90, true, 7)]
		[TestCase(Rotation.Rotate270, false, 8)]
		public void RotationTransform(Rotation rotation, bool isFlipped, int? orientation) {
			var path = Path.Combine(_testDataDir, "image4.jpg");
			var image = ImageSourceCreator.Create(path, orientation);
			if (isFlipped) {
				var tb = image as TransformedBitmap;
				Assert.NotNull(tb);
				Assert.AreEqual(-1, tb.Transform.Value.M11);
				Assert.AreEqual(1, tb.Transform.Value.M22);
				Assert.AreEqual(rotation, ((BitmapImage)tb.Source).Rotation);
			} else {
				var bi = image as BitmapImage;
				Assert.NotNull(bi);
				Assert.AreEqual(rotation, bi.Rotation);
			}

			Assert.That(image.IsFrozen);
		}

		[TestCase(640, 480, 0, 0)]
		[TestCase(320, 240, 320, 240)]
		[TestCase(640, 480, 640, 480)]
		[TestCase(100, 100, 100, 100)]
		public void PixelWidthHeight(int resultWidth, int resultHeight, double limitWidth, double limitHeight) {
			var path = Path.Combine(_testDataDir, "image4.jpg"); // 640x480
			var image = (BitmapImage)ImageSourceCreator.Create(path, 1, limitWidth, limitHeight);

			Assert.AreEqual(resultWidth, image.PixelWidth);
			Assert.AreEqual(resultHeight, image.PixelHeight);
		}

		[Test]
		public void Source() {
			var path = Path.Combine(_testDataDir, "image4.jpg"); // 640x480
			var image = ImageSourceCreator.Create(path);
			Assert.NotNull(image);
			var uri = new Uri(path);
			image = ImageSourceCreator.Create(uri);
			Assert.NotNull(image);
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			image = ImageSourceCreator.Create(stream);
			Assert.NotNull(image);

			Assert.Catch<ArgumentException>(() => {
				ImageSourceCreator.Create(5);
			});
		}
	}
}
