using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Tests.Models.Album;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileCollectionTest : TestClassBase{
		[Test]
		public void Items() {
			using (var mc = Get.Instance<MediaFileCollection>()) {
				Assert.AreEqual(0, mc.Count.Value);
				var item1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				mc.Items.Add(item1);
				Assert.AreEqual(1, mc.Count.Value);
				Assert.AreEqual(item1, mc.Items.First());
			}
		}
	}
}
