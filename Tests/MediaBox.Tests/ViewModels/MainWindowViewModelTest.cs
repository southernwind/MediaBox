using NUnit.Framework;

using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Album.Filter;

namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class MainWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var vm = new MainWindowViewModel();
			vm.AlbumSelectorViewModel.IsInstanceOf<AlbumSelectorViewModel>();
			vm.NavigationMenuViewModel.IsInstanceOf<NavigationMenuViewModel>();
			vm.FilterDescriptionManager.IsInstanceOf<FilterDescriptionManagerViewModel>();
			vm.TaskQueue.IsInstanceOf<PriorityTaskQueue>();
		}
	}
}
