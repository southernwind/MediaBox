using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views.Map;

namespace SandBeige.MediaBox.Tests.Views.Map {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class PinControlTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new PinControl();
		}
	}
}
