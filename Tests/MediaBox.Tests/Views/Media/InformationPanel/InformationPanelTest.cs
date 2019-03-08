using System.Threading;

using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views.Media.InformationWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class InformationPanelTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.Media.InformationPanel.InformationPanel();
		}
	}
}
