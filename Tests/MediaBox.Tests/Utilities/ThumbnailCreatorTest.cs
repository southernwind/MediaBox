using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Utilities {
	[TestFixture]
	internal class ThumbnailCreatorTest {

		[Test]
		public void CreateFromImage() {
			ThumbnailCreator.Create((Image)null, 500, 500, 0).Should().BeNull();

			// 正方形
			var image = new Bitmap(500, 500);
			var thumbnailImage = ThumbnailCreator.Create(image, 50, 100, 0);
			thumbnailImage.Width.Should().Be(50);
			thumbnailImage.Height.Should().Be(50);
			thumbnailImage = ThumbnailCreator.Create(image, 300, 150, 0);
			thumbnailImage.Width.Should().Be(150);
			thumbnailImage.Height.Should().Be(150);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 100, 0);
			thumbnailImage.Width.Should().Be(100);
			thumbnailImage.Height.Should().Be(100);
			thumbnailImage = ThumbnailCreator.Create(image, 300, 700, 0);
			thumbnailImage.Width.Should().Be(300);
			thumbnailImage.Height.Should().Be(300);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 700, 0);
			thumbnailImage.Width.Should().Be(500);
			thumbnailImage.Height.Should().Be(500);

			// 縦長
			image = new Bitmap(100, 500);
			thumbnailImage = ThumbnailCreator.Create(image, 50, 100, 0);
			thumbnailImage.Width.Should().Be(20);
			thumbnailImage.Height.Should().Be(100);
			thumbnailImage = ThumbnailCreator.Create(image, 50, 350, 0);
			thumbnailImage.Width.Should().Be(50);
			thumbnailImage.Height.Should().Be(250);
			thumbnailImage = ThumbnailCreator.Create(image, 200, 100, 0);
			thumbnailImage.Width.Should().Be(20);
			thumbnailImage.Height.Should().Be(100);
			thumbnailImage = ThumbnailCreator.Create(image, 70, 700, 0);
			thumbnailImage.Width.Should().Be(70);
			thumbnailImage.Height.Should().Be(350);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 700, 0);
			thumbnailImage.Width.Should().Be(100);
			thumbnailImage.Height.Should().Be(500);

			// 横長
			image = new Bitmap(500, 100);
			thumbnailImage = ThumbnailCreator.Create(image, 100, 50, 0);
			thumbnailImage.Width.Should().Be(100);
			thumbnailImage.Height.Should().Be(20);
			thumbnailImage = ThumbnailCreator.Create(image, 350, 50, 0);
			thumbnailImage.Width.Should().Be(250);
			thumbnailImage.Height.Should().Be(50);
			thumbnailImage = ThumbnailCreator.Create(image, 100, 200, 0);
			thumbnailImage.Width.Should().Be(100);
			thumbnailImage.Height.Should().Be(20);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 70, 0);
			thumbnailImage.Width.Should().Be(350);
			thumbnailImage.Height.Should().Be(70);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 700, 0);
			thumbnailImage.Width.Should().Be(500);
			thumbnailImage.Height.Should().Be(100);

		}

		[Test]
		public void CreateFromStream() {
			ThumbnailCreator.Create((Stream)null, 500, 500, 0).Should().BeNull();

			// 正方形
			var image = new Bitmap(500, 500);
			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Jpeg);
				var thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 100, 0)));

				thumbnailImage.Width.Should().Be(50);
				thumbnailImage.Height.Should().Be(50);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 300, 150, 0)));
				thumbnailImage.Width.Should().Be(150);
				thumbnailImage.Height.Should().Be(150);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 100, 0)));
				thumbnailImage.Width.Should().Be(100);
				thumbnailImage.Height.Should().Be(100);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 300, 700, 0)));
				thumbnailImage.Width.Should().Be(300);
				thumbnailImage.Height.Should().Be(300);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 700, 0)));
				thumbnailImage.Width.Should().Be(500);
				thumbnailImage.Height.Should().Be(500);
			}

			// 縦長
			image = new Bitmap(100, 500);
			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Gif);
				var thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 100, 0)));
				thumbnailImage.Width.Should().Be(20);
				thumbnailImage.Height.Should().Be(100);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 350, 0)));
				thumbnailImage.Width.Should().Be(50);
				thumbnailImage.Height.Should().Be(250);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 200, 100, 0)));
				thumbnailImage.Width.Should().Be(20);
				thumbnailImage.Height.Should().Be(100);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 70, 700, 0)));
				thumbnailImage.Width.Should().Be(70);
				thumbnailImage.Height.Should().Be(350);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 700, 0)));
				thumbnailImage.Width.Should().Be(100);
				thumbnailImage.Height.Should().Be(500);
			}

			// 横長
			image = new Bitmap(500, 100);

			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Png);
				var thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 100, 50, 0)));
				thumbnailImage.Width.Should().Be(100);
				thumbnailImage.Height.Should().Be(20);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 350, 50, 0)));
				thumbnailImage.Width.Should().Be(250);
				thumbnailImage.Height.Should().Be(50);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 100, 200, 0)));
				thumbnailImage.Width.Should().Be(100);
				thumbnailImage.Height.Should().Be(20);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 70, 0)));
				thumbnailImage.Width.Should().Be(350);
				thumbnailImage.Height.Should().Be(70);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 700, 0)));
				thumbnailImage.Width.Should().Be(500);
				thumbnailImage.Height.Should().Be(100);
			}
		}
	}
}
