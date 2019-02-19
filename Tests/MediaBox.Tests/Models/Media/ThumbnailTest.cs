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
			var thumbnail1 = new Thumbnail("image1.jpg");
			thumbnail1.FilePath.Is(Path.Combine(TestDirectories["3"], "image1.jpg"));
		}
	}
}
