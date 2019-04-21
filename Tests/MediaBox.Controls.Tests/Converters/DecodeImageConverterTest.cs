using System;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using NUnit.Framework;

using SandBeige.MediaBox.Controls.Converters;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class DecodeImageConverterTest : TestClassBase {

		[Test]
		public void Convert() {
			var stream = new FileStream(this.TestFiles.Image1Jpg.FilePath, FileMode.Open, FileAccess.Read);
			var converter = new DecodeImageConverter();
			var image = (BitmapImage)converter.Convert(new object[] { this.TestFiles.Image1Jpg.FilePath, 0 }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(7);
			image.Height.Is(5);

			image = (BitmapImage)converter.Convert(new object[] { stream, 0 }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(7);
			image.Height.Is(5);

			image = (BitmapImage)converter.Convert(new object[] { this.TestFiles.Image1Jpg.FilePath, 6 }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(5);
			image.Height.Is(7);

			image = (BitmapImage)converter.Convert(new object[] { this.TestFiles.Image1Jpg.FilePath, 0, 300d, 600d }, typeof(ImageSource), null, CultureInfo.InvariantCulture);
			image.Width.Is(300);
			image.Height.Is(600);

			converter.Convert(new object[] { 5, 0 }, typeof(ImageSource), null, CultureInfo.InvariantCulture).IsNull();
		}

		[Test]
		public void ConvertBack() {
			var converter = new DecodeImageConverter();
			Assert.Throws<NotSupportedException>(() => {
				converter.ConvertBack(null, new[] { typeof(bool) }, null, CultureInfo.InvariantCulture);
			});
		}
	}
}
