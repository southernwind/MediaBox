using System.Threading;
using NUnit.Framework;
using SandBeige.MediaBox.Views.SubWindows.OptionWindow.Pages;

namespace SandBeige.MediaBox.Tests.Views.SubWindows.OptionWindow.Pages {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class PathSettingsTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new PathSettings();
		}
	}
}
