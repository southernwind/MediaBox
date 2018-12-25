using System.Threading;

using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views.Media.InformationWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class InformationWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.Media.InformationWindow.InformationWindow();
		}
	}
}
