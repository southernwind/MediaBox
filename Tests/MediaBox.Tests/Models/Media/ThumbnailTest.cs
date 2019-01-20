using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class ThumbnailTest : TestClassBase {
		public void CreateThumbnail() {
			var settings = Get.Instance<ISettings>();
			settings.PathSettings.ThumbnailDirectoryPath.Value = TestDirectories["3"];

			// サムネイルファイル名直接指定
			var thumbnail1 = new Thumbnail {
				FileName = "image1.jpg"
			};
			thumbnail1.CreateThumbnail(ThumbnailLocation.File);
			thumbnail1.FilePath.Is(Path.Combine(TestDirectories["3"], "image1.jpg"));
			thumbnail1.FileName.Is("image1.jpg");
			thumbnail1.Location.Is(ThumbnailLocation.File);
			thumbnail1.Orientation.IsNull();

			// 生成元画像指定 ファイル生成→メモリ生成
			var thumbnail2 = new Thumbnail {
				FullSizeFilePath = Path.Combine(TestDataDir, "image1.jpg")
			};
			thumbnail2.CreateThumbnail(ThumbnailLocation.File);
			thumbnail2.FileName.IsNotNull();
			thumbnail2.FilePath.Is(Path.Combine(TestDirectories["3"], thumbnail2.FileName));
			thumbnail2.Location.Is(ThumbnailLocation.File);
			thumbnail2.Orientation.IsNull();
			thumbnail2.CreateThumbnail(ThumbnailLocation.Memory);
			thumbnail2.FileName.IsNotNull();
			thumbnail2.FilePath.Is(Path.Combine(TestDirectories["3"], thumbnail2.FileName));
			thumbnail2.Location.Is(ThumbnailLocation.File | ThumbnailLocation.Memory);
			thumbnail2.Orientation.IsNull();

			// 生成元画像指定 メモリ生成→ファイル生成
			var thumbnail3 = new Thumbnail {
				FullSizeFilePath = Path.Combine(TestDataDir, "image1.jpg")
			};
			thumbnail3.CreateThumbnail(ThumbnailLocation.Memory);
			thumbnail3.FileName.IsNull();
			thumbnail3.FilePath.IsNull();
			thumbnail3.Location.Is(ThumbnailLocation.Memory);
			thumbnail3.Orientation.IsNull();
			thumbnail3.CreateThumbnail(ThumbnailLocation.File);
			thumbnail3.FileName.IsNotNull();
			thumbnail3.FilePath.Is(Path.Combine(TestDirectories["3"], thumbnail2.FileName));
			thumbnail3.Location.Is(ThumbnailLocation.File | ThumbnailLocation.Memory);
			thumbnail3.Orientation.IsNull();

		}
	}
}
