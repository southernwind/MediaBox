using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Image;

namespace SandBeige.MediaBox.Library.Tests.Image {
	[TestFixture]
	internal class ThumbnailCreatorTest {
		[Test]
		public void CreateFromStream() {
			// 正方形
			var image = new Bitmap(500, 500);
			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Jpeg);
				ms.Position = 0;
				var thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 100)));

				thumbnailImage.Width.Should().Be(50);
				thumbnailImage.Height.Should().Be(50);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 300, 150)));
				thumbnailImage.Width.Should().Be(150);
				thumbnailImage.Height.Should().Be(150);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 100)));
				thumbnailImage.Width.Should().Be(100);
				thumbnailImage.Height.Should().Be(100);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 300, 700)));
				thumbnailImage.Width.Should().Be(300);
				thumbnailImage.Height.Should().Be(300);
			}

			// 縦長
			image = new Bitmap(100, 500);
			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Gif);
				ms.Position = 0;
				var thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 100)));
				thumbnailImage.Width.Should().Be(20);
				thumbnailImage.Height.Should().Be(100);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 350)));
				thumbnailImage.Width.Should().Be(50);
				thumbnailImage.Height.Should().Be(250);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 200, 100)));
				thumbnailImage.Width.Should().Be(20);
				thumbnailImage.Height.Should().Be(100);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 70, 700)));
				thumbnailImage.Width.Should().Be(70);
				thumbnailImage.Height.Should().Be(350);
			}

			// 横長
			image = new Bitmap(500, 100);

			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Png);
				ms.Position = 0;
				var thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 100, 50)));
				thumbnailImage.Width.Should().Be(100);
				thumbnailImage.Height.Should().Be(20);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 350, 50)));
				thumbnailImage.Width.Should().Be(250);
				thumbnailImage.Height.Should().Be(50);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 100, 200)));
				thumbnailImage.Width.Should().Be(100);
				thumbnailImage.Height.Should().Be(20);
				ms.Position = 0;
				thumbnailImage = System.Drawing.Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 70)));
				thumbnailImage.Width.Should().Be(350);
				thumbnailImage.Height.Should().Be(70);
			}
		}
	}
}
