using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views.Media.Detail;

namespace SandBeige.MediaBox.Tests.Views.Media.Detail {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MediaListTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaList();
		}
	}
}
