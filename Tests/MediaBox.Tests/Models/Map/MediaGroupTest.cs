using System.IO;
using System.Windows;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture]
	internal class MediaGroupTest : TestClassBase {
		[Test]
		public void Test() {
			var image1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
			var rect = new Rectangle(new Point(5, 15), new Size(10, 8));
			var mg = Get.Instance<MediaGroup>(image1, rect);

			mg.Core.Value.Is(image1);
			mg.CoreRectangle.Is(rect);
			mg.Items.Count.Is(1);
			mg.Items[0].Is(image1);
		}
	}
}
