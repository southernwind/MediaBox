using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileTest : ModelTestClassBase {

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
			using (var media = this.MediaFactory.Create(path)) {
				media.Thumbnail.IsNull();
				media.CreateThumbnailIfNotExists();
				media.Thumbnail.IsNotNull();
			}
			path = Path.Combine(TestDataDir, "image2.jpg");
			using (var media = this.MediaFactory.Create(path)) {
				media.Thumbnail.IsNull();
				media.CreateThumbnailIfNotExists();
				media.Thumbnail.IsNotNull();
			}
		}

		[Test]
		public void RegisterLoadDataBase() {
			var path = Path.Combine(TestDataDir, "image1.jpg");
			var db = Get.Instance<MediaBoxDbContext>();
			using (var media = (ImageFileModel)this.MediaFactory.Create(path)) {
				media.Thumbnail.IsNull();
				media.CreateThumbnail();
				media.Thumbnail.IsNotNull();
				media.Orientation = 3;
				media.Location = new GpsLocation(38.856, 66.431);
				media.Resolution = new ComparableSize(50, 80);
				media.CreateDataBaseRecord();
			}

			using (var media = (ImageFileModel)this.MediaFactory.Create(path)) {
				media.LoadFromDataBase();
				media.Location.Latitude.Is(38.856);
				media.Location.Longitude.Is(66.431);
				media.Thumbnail.IsNotNull();
				media.Resolution.Value.Width.Is(50);
				media.Resolution.Value.Height.Is(80);
				media.Orientation.Is(3);
			}

			using (var media = (ImageFileModel)this.MediaFactory.Create(path)) {
				media.LoadFromDataBase(db.MediaFiles.Single(x => x.FilePath == media.FilePath));
				media.Location.Latitude.Is(38.856);
				media.Location.Longitude.Is(66.431);
				media.Thumbnail.IsNotNull();
				media.Resolution.Value.Width.Is(50);
				media.Resolution.Value.Height.Is(80);
				media.Orientation.Is(3);
			}
		}

		[Test]
		public void LoadImageUnloadImage() {
			var path1 = Path.Combine(TestDataDir, "image1.jpg");
			var path2 = Path.Combine(TestDataDir, "image2.jpg");
			using (var media1 = (ImageFileModel)this.MediaFactory.Create(path1))
			using (var media2 = (ImageFileModel)this.MediaFactory.Create(path2)) {
				media1.Image.IsNull();
				media1.Orientation = 1;
				media1.LoadImage();
				media1.Image.IsNotNull();
				((BitmapSource)media1.Image).PixelHeight.Is(3024);
				((BitmapSource)media1.Image).PixelWidth.Is(4032);

				media2.Image.IsNull();
				media2.Orientation = 6;
				media2.LoadImage();
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
