using System.Threading;
using NUnit.Framework;
using SandBeige.MediaBox.Views.Media.Map;

namespace SandBeige.MediaBox.Tests.Views.Media.Map {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MediaListTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaList();
		}
	}
}
