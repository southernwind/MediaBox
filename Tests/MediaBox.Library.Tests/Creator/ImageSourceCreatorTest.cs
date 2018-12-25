using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Creator;

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
		public async Task RotationTransform(Rotation rotation, bool isFlipped, int? orientation) {
			var path = Path.Combine(_testDataDir, "image4.jpg");
			var image = ImageSourceCreator.Create(path, orientation);
			var image2 = await ImageSourceCreator.CreateAsync(path, orientation);
			if (isFlipped) {
				var tb = image as TransformedBitmap;
				var tb2 = image2 as TransformedBitmap;
				tb.IsNotNull();
				tb2.IsNotNull();
				tb.Transform.Value.M11.Is(-1);
				tb2.Transform.Value.M11.Is(-1);
				tb.Transform.Value.M22.Is(1);
				tb2.Transform.Value.M22.Is(1);
				((BitmapImage)tb.Source).Rotation.Is(rotation);
				((BitmapImage)tb2.Source).Rotation.Is(rotation);
			} else {
				var bi = image as BitmapImage;
				var bi2 = image2 as BitmapImage;
				bi.IsNotNull();
				bi2.IsNotNull();
				bi.Rotation.Is(rotation);
				bi2.Rotation.Is(rotation);
			}

			image.IsFrozen.IsTrue();
			image2.IsFrozen.IsTrue();
		}

		[TestCase(640, 480, 0, 0)]
		[TestCase(320, 240, 320, 240)]
		[TestCase(640, 480, 640, 480)]
		[TestCase(100, 100, 100, 100)]
		public async Task PixelWidthHeight(int resultWidth, int resultHeight, double limitWidth, double limitHeight) {
			var path = Path.Combine(_testDataDir, "image4.jpg"); // 640x480
			var image = (BitmapImage)ImageSourceCreator.Create(path, 1, limitWidth, limitHeight);
			var image2 = (BitmapImage)await ImageSourceCreator.CreateAsync(path, 1, limitWidth, limitHeight);
			image.PixelWidth.Is(resultWidth);
			image2.PixelWidth.Is(resultWidth);
			image.PixelHeight.Is(resultHeight);
			image2.PixelHeight.Is(resultHeight);
		}

		[Test]
		public async Task Source() {
			var path = Path.Combine(_testDataDir, "image4.jpg"); // 640x480
			var image = ImageSourceCreator.Create(path);
			var image2 = await ImageSourceCreator.CreateAsync(path);
			image.IsNotNull();
			image2.IsNotNull();
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			image = ImageSourceCreator.Create(stream);
			image2 = await ImageSourceCreator.CreateAsync(path);
			image.IsNotNull();
			image2.IsNotNull();

			Assert.Catch<ArgumentException>(() => {
				ImageSourceCreator.Create(5);
			});
		}

		[Test]
		public async Task CancellationToken() {
			var path = Path.Combine(_testDataDir, "image4.jpg"); // 640x480
			var token = new CancellationTokenSource();
			var token2 = new CancellationTokenSource();
			var task = ImageSourceCreator.CreateAsync(path, token: token.Token);
			var task2 = ImageSourceCreator.CreateAsync(path, token: token2.Token);
			token.Cancel();
			(await task).IsNull();
			(await task2).IsNotNull();
		}
	}
}
