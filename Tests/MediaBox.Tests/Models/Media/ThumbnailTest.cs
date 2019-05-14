
using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Tests.Models.Media {
	internal class ThumbnailTest : ModelTestClassBase {

		[Test]
		public void サムネイルファイル名生成() {
			var s = Thumbnail.GetThumbnailFileName(@"C:\test\thumb.jpg");
			s.Is(@"A0\0C07F8E9A92696F73E19002FBEB4CA1639CE56B3EF1598957B37D3D1499FAD");
		}
	}
}
