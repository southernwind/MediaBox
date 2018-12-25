using System;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using NUnit.Framework;

using SandBeige.MediaBox.Controls.Converters;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class DecodeImageConverterTest {
		private static string _testDataDir;

		[SetUp]
		public void SetUp() {
			_testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
		}
		[Test]
		public void Convert() {
			var path = Path.Combine(_testDataDir, "image4.jpg"); // 640x480
			var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			var converter = new DecodeImageConverter();
			var image = (BitmapImage)converter.Convert(new object[] {path, 0}, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(640);
			image.Height.Is(480);

			image = (BitmapImage)converter.Convert(new object[] { stream, 0 }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(640);
			image.Height.Is(480);

			image = (BitmapImage)converter.Convert(new object[] { path, 6 }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(480);
			image.Height.Is(640);

			image = (BitmapImage)converter.Convert(new object[] { path, 0, 300d, 600d }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(300);
			image.Height.Is(600);

			converter.Convert(new object[] { 5, 0 }, typeof(ImageSource), null, CultureInfo.InvariantCulture).IsNull();
		}

		[Test]
		public void ConvertBack() {
			var converter = new DecodeImageConverter();
			Assert.Throws<NotImplementedException>(() => {
				converter.ConvertBack(null, new[] { typeof(bool) }, null, CultureInfo.InvariantCulture);
			});
		}
	}
}
