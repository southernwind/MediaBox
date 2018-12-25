using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Tests.Views;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class PinTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new Pin();
		}
	}
}
