using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Image;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Library.Tests.Image {
	internal class MetadataTest {
		internal class FFmpegTest : TestClassBase {
			[Test]
			public void WidthHeight() {
				using (var png = ImageMetadataFactory.Create(File.OpenRead(this.TestFiles.Image1Jpg.FilePath))) {
					png.Width.Is(7);
					png.Height.Is(5);
				}
				using (var jpg1 = ImageMetadataFactory.Create(File.OpenRead(this.TestFiles.Image2Jpg.FilePath))) {
					jpg1.Width.Is(5);
					jpg1.Height.Is(5);
				}
				using (var jpg2 = ImageMetadataFactory.Create(File.OpenRead(this.TestFiles.Image3Jpg.FilePath))) {
					jpg2.Width.Is(4);
					jpg2.Height.Is(4);
				}
			}
		}
	}
}
