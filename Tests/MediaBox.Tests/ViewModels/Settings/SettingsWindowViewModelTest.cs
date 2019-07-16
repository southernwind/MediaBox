using NUnit.Framework;

using SandBeige.MediaBox.ViewModels.Settings;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings {
	[TestFixture]
	internal class SettingsWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			using var _ = new SettingsWindowViewModel();
		}
	}
}
