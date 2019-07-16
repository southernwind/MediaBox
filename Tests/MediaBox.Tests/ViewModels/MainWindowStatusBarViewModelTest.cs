using NUnit.Framework;

using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class MainWindowStatusBarViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			using var vm = new MainWindowStatusBarViewModel();
		}
	}
}
