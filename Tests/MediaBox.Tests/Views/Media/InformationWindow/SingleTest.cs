using System.Threading;

using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views.Media.InformationWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class SingleTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.Media.InformationWindow.Single();
		}
	}
}
