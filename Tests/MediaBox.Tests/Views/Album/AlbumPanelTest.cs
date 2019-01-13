using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views.Album;

namespace SandBeige.MediaBox.Tests.Views.Album {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class AlbumPanelTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new AlbumPanel();
		}
	}
}
