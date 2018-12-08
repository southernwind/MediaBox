using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class ThumbnailTest :TestClassBase {
		[TestCase("image1.jpg")]
		[TestCase("image1.png")]
		[TestCase("image1.heic")]
		public void ThumbnailString(string param) {
			var settings = Get.Instance<ISettings>();
			settings.PathSettings.ThumbnailDirectoryPath.Value = TestDirectories["3"];

			var thumbnail = new Thumbnail(param);
			Assert.AreEqual(thumbnail.FilePath, Path.Combine(TestDirectories["3"], param));
			Assert.AreEqual(thumbnail.FileName, param);
			Assert.IsNull(thumbnail.ImageStream);
			Assert.IsNull(thumbnail.Orientation);
			Assert.IsNull(thumbnail.Image);
			thumbnail.Orientation = 5;
			Assert.AreEqual(5, thumbnail.Orientation);
			Assert.AreEqual(thumbnail.FilePath, thumbnail.Source);
		}

		[TestCase(new byte[] { 1, 3, 45, 222, 6, 1, 71, 71, 23, 13, 132, 251 })]
		[TestCase(new byte[] { })]
		public void ThumbnailByteArray(byte[] param) {
			var settings = Get.Instance<ISettings>();
			settings.PathSettings.ThumbnailDirectoryPath.Value = TestDirectories["3"];

			var thumbnail = new Thumbnail(param);
			Assert.IsNull(thumbnail.FilePath);
			Assert.IsNull(thumbnail.FileName);
			CollectionAssert.AreEqual(((MemoryStream)thumbnail.ImageStream).ToArray(), param);
			Assert.IsNull(thumbnail.Orientation);
			CollectionAssert.AreEqual(thumbnail.Image, param);
			thumbnail.Orientation = 5;
			Assert.AreEqual(5, thumbnail.Orientation);
			Assert.AreEqual(thumbnail.ImageStream, thumbnail.Source);
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
