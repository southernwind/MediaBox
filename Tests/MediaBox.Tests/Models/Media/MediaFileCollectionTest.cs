using System.IO;
using System.Linq;
using System.Reactive.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileCollectionTest : TestClassBase {
		[Test]
		public void Items() {
			using (var mc = Get.Instance<MediaFileCollection>()) {
				mc.Count.Value.Is(0);
				var item1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
				mc.Items.Add(item1);
				mc.Count.Value.Is(1);
				mc.Items.First().Is(item1);
			}
		}
	}
}
