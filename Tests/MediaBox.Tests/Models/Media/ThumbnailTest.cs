using System;
using System.IO;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class ThumbnailTest : TestClassBase {
		[TestCase("image1.jpg")]
		[TestCase("image1.png")]
		[TestCase("image1.heic")]
		public void ThumbnailString(string param) {
			var settings = Get.Instance<ISettings>();
			settings.PathSettings.ThumbnailDirectoryPath.Value = TestDirectories["3"];

			var thumbnail = new Thumbnail(param);
			Path.Combine(TestDirectories["3"], param).Is(thumbnail.FilePath);
			thumbnail.FileName.Is(param);
			thumbnail.ImageStream.IsNull();
			thumbnail.Orientation.IsNull();
			thumbnail.Image.IsNull();
			thumbnail.Orientation = 5;
			thumbnail.Orientation.Is(5);
			thumbnail.FilePath.Is(thumbnail.Source);
		}

		[TestCase(new byte[] { 1, 3, 45, 222, 6, 1, 71, 71, 23, 13, 132, 251 })]
		[TestCase(new byte[] { })]
		public void ThumbnailByteArray(byte[] param) {
			var settings = Get.Instance<ISettings>();
			settings.PathSettings.ThumbnailDirectoryPath.Value = TestDirectories["3"];

			var thumbnail = new Thumbnail(param);
			thumbnail.FilePath.IsNull();
			thumbnail.FileName.IsNull();
			((MemoryStream)thumbnail.ImageStream).ToArray().Is(param);
			thumbnail.Orientation.IsNull();
			thumbnail.Image.Is(param);
			thumbnail.Orientation = 5;
			thumbnail.Orientation.Is(5);
			thumbnail.ImageStream.Is(thumbnail.Source);
		}

		[TestCase((byte)5)]
		[TestCase('a')]
		[TestCase(typeof(string))]
		public void ThumbnailOther(object param) {
			Assert.Catch<ArgumentException>(() => {
				_ = new Thumbnail(param);
			});
		}
	}
}
