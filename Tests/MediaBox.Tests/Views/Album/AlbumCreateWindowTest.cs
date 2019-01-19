using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views.Album;

namespace SandBeige.MediaBox.Tests.Views.Album {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class AlbumCreateWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new AlbumCreateWindow();
		}
	}
}
