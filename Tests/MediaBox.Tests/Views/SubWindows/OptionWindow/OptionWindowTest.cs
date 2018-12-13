using System.Threading;
using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.Views.SubWindows.OptionWindow {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class OptionWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new MediaBox.Views.SubWindows.OptionWindow.OptionWindow();
		}
	}
}
