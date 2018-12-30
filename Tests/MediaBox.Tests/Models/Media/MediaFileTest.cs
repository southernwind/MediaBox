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
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.FilePath.Value.Is(path);
				media.FileName.Value.Is("image1.jpg");
			}

			path = Path.Combine(TestDirectories["0"], "image2.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.FilePath.Value.Is(path);
				media.FileName.Value.Is("image2.jpg");
			}
		}

		[Test]
		public void CreateThumbnail() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			var pool = Get.Instance<ThumbnailPool>();
			using (var media = this.MediaFactory.Create(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				media.CreateThumbnail(ThumbnailLocation.Memory);
				pool.Resolve(path).IsNotNull();
				media.Thumbnail.Value.IsNotNull();
				pool.Resolve(path).Is(media.Thumbnail.Value.Image);
			}
			path = Path.Combine(TestDirectories["0"], "image2.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				media.CreateThumbnail(ThumbnailLocation.Memory);
				pool.Resolve(path).IsNotNull();
				media.Thumbnail.Value.IsNotNull();
				pool.Resolve(path).Is(media.Thumbnail.Value.Image);
			}
			path = Path.Combine(TestDirectories["0"], "image3.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				media.CreateThumbnail(ThumbnailLocation.File);
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNotNull();
				media.Thumbnail.Value.FilePath.IsNotNull();
				using (var fs = new FileStream(media.Thumbnail.Value.FilePath, FileMode.Open)) {
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.StreamSource = fs;
					bitmap.EndInit();
					bitmap.Freeze();
				}
			}
			path = Path.Combine(TestDirectories["0"], "image4.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				media.CreateThumbnail(ThumbnailLocation.File);
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNotNull();
				media.Thumbnail.Value.FilePath.IsNotNull();
				using (var fs = new FileStream(media.Thumbnail.Value.FilePath, FileMode.Open)) {
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.StreamSource = fs;
					bitmap.EndInit();
					bitmap.Freeze();
				}
			}
		}

		[Test]
		public void RegisterLoadDataBase() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			var db = Get.Instance<MediaBoxDbContext>();
			using (var media = this.MediaFactory.Create(path)) {
				media.Latitude.Value = 38.856;
				media.Longitude.Value = 66.431;
				media.Thumbnail.Value = Get.Instance<Thumbnail>(path + "_thumb");
				media.Orientation.Value = 3;
				media.RegisterToDataBase();
			}

			using (var media = this.MediaFactory.Create(path)) {
				media.LoadFromDataBase();
				media.Latitude.Value.Is(38.856);
				media.Longitude.Value.Is(66.431);
				media.Thumbnail.Value.FilePath.Is(path + "_thumb");
				media.Orientation.Value.Is(3);
			}

			using (var media = this.MediaFactory.Create(path)) {
				media.LoadFromDataBase(db.MediaFiles.Single(x => Path.Combine(x.DirectoryPath, x.FileName) == media.FilePath.Value));
				media.Latitude.Value.Is(38.856);
				media.Longitude.Value.Is(66.431);
				media.Thumbnail.Value.FilePath.Is(path + "_thumb");
				media.Orientation.Value.Is(3);
			}
		}

		[Test]
		public void LoadExifIfNotLoaded() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.Exif.Value.IsNull();
				media.LoadExifIfNotLoaded();
				media.Exif.Value.IsNotNull();
				media.LoadExifIfNotLoaded();
				media.Exif.Value.IsNotNull();
			}
		}

		[Test]
		public void LoadExif() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.Exif.Value.IsNull();
				media.LoadExif();
				media.Exif.Value.IsNotNull();
				Assert.AreEqual(35.6517139, media.Latitude.Value, 0.00001);
				Assert.AreEqual(136.821275, media.Longitude.Value, 0.00001);
				media.Orientation.Value.Is(1);
			}
		}

		[Test]
		public void SetGps() {
			var path1 = Path.Combine(TestDirectories["0"], "image1.jpg");
			var path2 = Path.Combine(TestDirectories["0"], "image2.jpg");
			var db = Get.Instance<MediaBoxDbContext>();
			using (var media = this.MediaFactory.Create(path1)) {
				media.Latitude.Value.IsNull();
				media.Longitude.Value.IsNull();

				// DB登録されていないmedia
				media.SetGps(50.15, 40.36);
				media.Latitude.Value.IsNull();
				media.Longitude.Value.IsNull();
			}

			using (var media1 = this.MediaFactory.Create(path1))
			using (var media2 = this.MediaFactory.Create(path2)) {
				media1.RegisterToDataBase();
				media2.RegisterToDataBase();

				var dbMedia1 = db.MediaFiles.Single(x => x.MediaFileId == media1.MediaFileId);
				var dbMedia2 = db.MediaFiles.Single(x => x.MediaFileId == media2.MediaFileId);

				media2.SetGps(50.15, 40.36);
				media1.SetGps(60.173, 62.44);

				dbMedia2.Latitude.Is(50.15);
				media2.Latitude.Value.Is(50.15);
				dbMedia2.Longitude.Is(40.36);
				media2.Longitude.Value.Is(40.36);

				dbMedia1.Latitude.Is(60.173);
				media1.Latitude.Value.Is(60.173);
				dbMedia1.Longitude.Is(62.44);
				media1.Longitude.Value.Is(62.44);
			}
		}

		[Test]
		public async Task LoadImageUnloadImage() {
			var path1 = Path.Combine(TestDirectories["0"], "image1.jpg");
			var path2 = Path.Combine(TestDirectories["0"], "image2.jpg");
			using (var media1 = this.MediaFactory.Create(path1))
			using (var media2 = this.MediaFactory.Create(path2)) {
				media1.Image.Value.IsNull();
				media1.Orientation.Value = 1;
				await media1.LoadImageAsync();
				media1.Image.Value.IsNotNull();
				((BitmapSource)media1.Image.Value).PixelHeight.Is(3024);
				((BitmapSource)media1.Image.Value).PixelWidth.Is(4032);

				media2.Image.Value.IsNull();
				media2.Orientation.Value = 6;
				await media2.LoadImageAsync();
				media2.Image.Value.IsNotNull();
				((BitmapSource)media2.Image.Value).PixelHeight.Is(4032);
				((BitmapSource)media2.Image.Value).PixelWidth.Is(3024);

				media1.UnloadImage();
				media1.Image.Value.IsNull();
				media2.UnloadImage();
				media2.Image.Value.IsNull();
			}
		}
	}
}
