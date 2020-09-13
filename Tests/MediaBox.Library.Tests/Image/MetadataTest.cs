using System.IO;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Image;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Library.Tests.Image {
	internal class MetadataTest {
		internal class FfmpegTest : TestClassBase {
			[Test]
			public void WidthHeight() {
				using (var png = ImageMetadataFactory.Create(File.OpenRead(this.TestFiles.Image1Jpg.FilePath))) {
					png.Width.Should().Be(7);
					png.Height.Should().Be(5);
				}
				using (var jpg1 = ImageMetadataFactory.Create(File.OpenRead(this.TestFiles.Image2Jpg.FilePath))) {
					jpg1.Width.Should().Be(5);
					jpg1.Height.Should().Be(5);
				}

				using var jpg2 = ImageMetadataFactory.Create(File.OpenRead(this.TestFiles.Image3Jpg.FilePath));
				jpg2.Width.Should().Be(4);
				jpg2.Height.Should().Be(4);
			}
		}
	}
}
