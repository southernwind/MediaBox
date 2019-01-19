using NUnit.Framework;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings.Pages {
	[TestFixture]
	internal class PathSettingsViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<GeneralSettingsViewModel>();
		}
	}
}
