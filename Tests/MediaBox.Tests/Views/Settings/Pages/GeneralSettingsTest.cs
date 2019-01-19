using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views.Settings.Pages;

namespace SandBeige.MediaBox.Tests.Views.Settings.Pages {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class GeneralSettingsTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new GeneralSettings();
		}
	}
}
