using NUnit.Framework;

using SandBeige.MediaBox.ViewModels.Settings.Pages;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings.Pages {
	[TestFixture]
	internal class PathSettingsViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			_ = new PathSettingsViewModel();
		}
	}
}
