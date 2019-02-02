using System;
using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Image;

namespace SandBeige.MediaBox.Library.Tests.Image {
	internal class MetadataTest {
		[TestFixture]
		internal class FFmpegTest {
			private static string _testDataDir;

			[SetUp]
			public void SetUp() {
				_testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
			}

			[Test]
			public void WidthHeight() {
				using (var png = new Metadata(File.OpenRead(Path.Combine(_testDataDir, "image2.png")))) {
					png.Width.Is(1366);
					png.Height.Is(768);
				}
				using (var jpg1 = new Metadata(File.OpenRead(Path.Combine(_testDataDir, "image3.jpg")))) {
					jpg1.Width.Is(4032);
					jpg1.Height.Is(3024);
				}
				using (var jpg2 = new Metadata(File.OpenRead(Path.Combine(_testDataDir, "image4.jpg")))) {
					jpg2.Width.Is(640);
					jpg2.Height.Is(480);
				}


			}
		}
	}
}
