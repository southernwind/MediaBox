using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Utilities {
	[TestFixture]
	internal class ThumbnailCreatorTest {

		[Test]
		public void CreateFromImage() {
			Assert.IsNull(ThumbnailCreator.Create((Image)null, 500, 500));

			// 正方形
			var image = new Bitmap(500,500);
			var thumbnailImage = ThumbnailCreator.Create(image, 50, 100);
			Assert.AreEqual(50, thumbnailImage.Width);
			Assert.AreEqual(50, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 300, 150);
			Assert.AreEqual(150, thumbnailImage.Width);
			Assert.AreEqual(150, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 100);
			Assert.AreEqual(100, thumbnailImage.Width);
			Assert.AreEqual(100, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 300, 700);
			Assert.AreEqual(300, thumbnailImage.Width);
			Assert.AreEqual(300, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 700);
			Assert.AreEqual(500, thumbnailImage.Width);
			Assert.AreEqual(500, thumbnailImage.Height);

			// 縦長
			image = new Bitmap(100, 500);
			thumbnailImage = ThumbnailCreator.Create(image, 50, 100);
			Assert.AreEqual(20, thumbnailImage.Width);
			Assert.AreEqual(100, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 50, 350);
			Assert.AreEqual(50, thumbnailImage.Width);
			Assert.AreEqual(250, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 200, 100);
			Assert.AreEqual(20, thumbnailImage.Width);
			Assert.AreEqual(100, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 70, 700);
			Assert.AreEqual(70, thumbnailImage.Width);
			Assert.AreEqual(350, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 700);
			Assert.AreEqual(100, thumbnailImage.Width);
			Assert.AreEqual(500, thumbnailImage.Height);

			// 横長
			image = new Bitmap(500, 100);
			thumbnailImage = ThumbnailCreator.Create(image, 100, 50);
			Assert.AreEqual(100, thumbnailImage.Width);
			Assert.AreEqual(20, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 350, 50);
			Assert.AreEqual(250, thumbnailImage.Width);
			Assert.AreEqual(50, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 100, 200);
			Assert.AreEqual(100, thumbnailImage.Width);
			Assert.AreEqual(20, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 70);
			Assert.AreEqual(350, thumbnailImage.Width);
			Assert.AreEqual(70, thumbnailImage.Height);
			thumbnailImage = ThumbnailCreator.Create(image, 700, 700);
			Assert.AreEqual(500, thumbnailImage.Width);
			Assert.AreEqual(100, thumbnailImage.Height);

		}

		[Test]
		public void CreateFromStream() {
			Assert.IsNull(ThumbnailCreator.Create((Stream)null, 500, 500));

			// 正方形
			var image = new Bitmap(500, 500);
			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Jpeg);
				var thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 100)));

				Assert.AreEqual(50, thumbnailImage.Width);
				Assert.AreEqual(50, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 300, 150)));
				Assert.AreEqual(150, thumbnailImage.Width);
				Assert.AreEqual(150, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 100)));
				Assert.AreEqual(100, thumbnailImage.Width);
				Assert.AreEqual(100, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 300, 700)));
				Assert.AreEqual(300, thumbnailImage.Width);
				Assert.AreEqual(300, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 700)));
				Assert.AreEqual(500, thumbnailImage.Width);
				Assert.AreEqual(500, thumbnailImage.Height);
			}

			// 縦長
			image = new Bitmap(100, 500);
			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Gif);
				var thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 100)));
				Assert.AreEqual(20, thumbnailImage.Width);
				Assert.AreEqual(100, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 50, 350)));
				Assert.AreEqual(50, thumbnailImage.Width);
				Assert.AreEqual(250, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 200, 100)));
				Assert.AreEqual(20, thumbnailImage.Width);
				Assert.AreEqual(100, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 70, 700)));
				Assert.AreEqual(70, thumbnailImage.Width);
				Assert.AreEqual(350, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 700)));
				Assert.AreEqual(100, thumbnailImage.Width);
				Assert.AreEqual(500, thumbnailImage.Height);
			}

			// 横長
			image = new Bitmap(500, 100);

			using (var ms = new MemoryStream()) {
				image.Save(ms, ImageFormat.Png);
				var thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 100, 50)));
				Assert.AreEqual(100, thumbnailImage.Width);
				Assert.AreEqual(20, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 350, 50)));
				Assert.AreEqual(250, thumbnailImage.Width);
				Assert.AreEqual(50, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 100, 200)));
				Assert.AreEqual(100, thumbnailImage.Width);
				Assert.AreEqual(20, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 70)));
				Assert.AreEqual(350, thumbnailImage.Width);
				Assert.AreEqual(70, thumbnailImage.Height);
				thumbnailImage = Image.FromStream(new MemoryStream(ThumbnailCreator.Create(ms, 700, 700)));
				Assert.AreEqual(500, thumbnailImage.Width);
				Assert.AreEqual(100, thumbnailImage.Height);
			}
		}
	}
}
