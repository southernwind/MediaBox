using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views.Media.InformationWindow;

namespace SandBeige.MediaBox.Tests.Views.Media.InformationWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MultipleTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new Multiple();
		}
	}
}
