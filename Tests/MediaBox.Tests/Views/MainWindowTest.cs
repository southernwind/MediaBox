using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views;

namespace SandBeige.MediaBox.Tests.Views {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class MainWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MainWindow();
		}
	}
}
