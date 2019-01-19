using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.Tests.Views.Settings {
	[TestFixture, Apartment(ApartmentState.STA)]
	internal class SettingsWindowTest : ViewTestClassBase {
		[Test]
		public void Test() {
			_ = new SettingsWindow();
		}
	}
}
