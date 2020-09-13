using System;
using System.Globalization;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Controls.Converters;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Controls.Tests.Converters {
	[TestFixture]
	internal class DecodeImageConverterTest : TestClassBase {

		[Test]
		public void Convert() {
			using var stream = new FileStream(this.TestFiles.Image1Jpg.FilePath, FileMode.Open, FileAccess.Read);
			var converter = new DecodeImageConverter();
			var image = (BitmapImage?)converter.Convert(new object[] { this.TestFiles.Image1Jpg.FilePath, 0 }, typeof(ImageSource), null!, CultureInfo.InvariantCulture)!;
			image!.Width.Should().Be(7);
			image.Height.Should().Be(5);

			image = (BitmapImage?)converter.Convert(new object[] { stream, 0 }, typeof(ImageSource), null!, CultureInfo.InvariantCulture)!;
			image!.Width.Should().Be(7);
			image.Height.Should().Be(5);

			image = (BitmapImage?)converter.Convert(new object[] { this.TestFiles.Image1Jpg.FilePath, 6 }, typeof(ImageSource), null!, CultureInfo.InvariantCulture)!;
			image!.Width.Should().Be(5);
			image.Height.Should().Be(7);

			image = (BitmapImage?)converter.Convert(new object[] { this.TestFiles.Image1Jpg.FilePath, 0, 300d, 600d }, typeof(ImageSource), null!, CultureInfo.InvariantCulture)!;
			image!.Width.Should().Be(300);
			image.Height.Should().Be(600);

			converter.Convert(new object[] { "err", 0 }, typeof(ImageSource), typeof(string), CultureInfo.InvariantCulture).Should().BeNull();
		}

		[Test]
		public void ConvertBack() {
			var converter = new DecodeImageConverter();
			Assert.Throws<NotSupportedException>(() => {
				converter.ConvertBack(null!, new[] { typeof(bool) }, typeof(string), CultureInfo.InvariantCulture);
			});
		}
	}
}
