using NUnit.Framework;

using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class MainWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			using var vm = new MainWindowViewModel();
			vm.AlbumSelectorViewModel.IsInstanceOf<AlbumSelectorViewModel>();
			vm.NavigationMenuViewModel.IsInstanceOf<NavigationMenuViewModel>();
			vm.MainWindowStatusBarViewModel.IsInstanceOf<MainWindowStatusBarViewModel>();
		}
	}
}
