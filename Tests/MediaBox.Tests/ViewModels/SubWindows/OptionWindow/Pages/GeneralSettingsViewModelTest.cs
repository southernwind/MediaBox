using NUnit.Framework;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages;

namespace SandBeige.MediaBox.Tests.ViewModels.SubWindows.OptionWindow.Pages {
	[TestFixture]
	internal class GeneralSettingsViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<GeneralSettingsViewModel>();
		}
	}
}
