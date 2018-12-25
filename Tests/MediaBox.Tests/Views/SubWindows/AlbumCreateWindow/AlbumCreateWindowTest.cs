using System.Threading;

using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views.SubWindows.AlbumCreateWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class AlbumCreateWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.SubWindows.AlbumCreateWindow.AlbumCreateWindow();
		}
	}
}
