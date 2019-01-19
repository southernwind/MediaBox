using NUnit.Framework;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Settings;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings {
	[TestFixture]
	internal class SettingsWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<SettingsWindowViewModel>();
		}
	}
}
