using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileTest : TestClassBase {

		[Test]
		public void MediaFile() {
			var path = Path.Combine(TestDataDir, "image1.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.FilePath.Is(path);
				media.FileName.Is("image1.jpg");
			}

			path = Path.Combine(TestDataDir, "image2.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.FilePath.Is(path);
				media.FileName.Is("image2.jpg");
			}
		}

		[Test]
		public void CreateThumbnail() {
			var path = Path.Combine(TestDataDir, "image1.jpg");
			var pool = Get.Instance<ThumbnailPool>();
			using (var media = this.MediaFactory.Create(path)) {
				media.Thumbnail.IsNull();
				media.CreateThumbnailIfNotExists(ThumbnailLocation.Memory);
				media.Thumbnail.Location.Is(ThumbnailLocation.Memory);
				media.CreateThumbnailIfNotExists(ThumbnailLocation.File);
				media.Thumbnail.Location.Is(ThumbnailLocation.File | ThumbnailLocation.Memory);
			}
			path = Path.Combine(TestDataDir, "image2.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.Thumbnail.IsNull();
				media.CreateThumbnailIfNotExists(ThumbnailLocation.File);
				media.Thumbnail.Location.Is(ThumbnailLocation.File);
				media.CreateThumbnailIfNotExists(ThumbnailLocation.Memory);
				media.Thumbnail.Location.Is(ThumbnailLocation.File | ThumbnailLocation.Memory);
			}
		}

		[Test]
		public void RegisterLoadDataBase() {
			var path = Path.Combine(TestDataDir, "image1.jpg");
			var db = Get.Instance<MediaBoxDbContext>();
			using (var media = this.MediaFactory.Create(path)) {
				media.Latitude = 38.856;
				media.Longitude = 66.431;
				media.Orientation = 3;
				media.Thumbnail.IsNull();
				media.CreateThumbnail(ThumbnailLocation.File);
				media.Thumbnail.IsNotNull();
				media.RegisterToDataBase();
			}

			using (var media = this.MediaFactory.Create(path)) {
				media.LoadFromDataBase();
				media.Latitude.Is(38.856);
				media.Longitude.Is(66.431);
				media.Thumbnail.Location.Is(ThumbnailLocation.File);
				media.Orientation.Is(3);
			}

			using (var media = this.MediaFactory.Create(path)) {
				media.LoadFromDataBase(db.MediaFiles.Single(x => x.FilePath == media.FilePath));
				media.Latitude.Is(38.856);
				media.Longitude.Is(66.431);
				media.Thumbnail.Location.Is(ThumbnailLocation.File);
				media.Orientation.Is(3);
			}
		}

		[Test]
		public void GetFileInfoIfNotLoaded() {
			var path = Path.Combine(TestDataDir, "image1.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.Exif.IsNull();
				media.GetFileInfoIfNotLoaded();
				media.Exif.IsNotNull();
				media.GetFileInfoIfNotLoaded();
				media.Exif.IsNotNull();
			}
		}

		[Test]
		public void GetFileInfo() {
			var path = Path.Combine(TestDataDir, "image1.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.Exif.IsNull();
				media.GetFileInfo();
				media.Exif.IsNotNull();
				Assert.AreEqual(35.6517139, media.Latitude, 0.00001);
				Assert.AreEqual(136.821275, media.Longitude, 0.00001);
				media.Orientation.Is(1);
			}
		}

		[Test]
		public async Task LoadImageUnloadImage() {
			var path1 = Path.Combine(TestDataDir, "image1.jpg");
			var path2 = Path.Combine(TestDataDir, "image2.jpg");
			using (var media1 = (ImageFileModel)this.MediaFactory.Create(path1))
			using (var media2 = (ImageFileModel)this.MediaFactory.Create(path2)) {
				media1.Image.IsNull();
				media1.Orientation = 1;
				await media1.LoadImageAsync();
				media1.Image.IsNotNull();
				((BitmapSource)media1.Image).PixelHeight.Is(3024);
				((BitmapSource)media1.Image).PixelWidth.Is(4032);

				media2.Image.IsNull();
				media2.Orientation = 6;
				await media2.LoadImageAsync();
				media2.Image.IsNotNull();
				((BitmapSource)media2.Image).PixelHeight.Is(4032);
				((BitmapSource)media2.Image).PixelWidth.Is(3024);

				media1.UnloadImage();
				media1.Image.IsNull();
				media2.UnloadImage();
				media2.Image.IsNull();
			}
		}
	}
}
