
using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	[TestFixture]
	internal class AlbumViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Title() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			vm.Title.Value.IsNull();
			model.Title.Value = "potato";
			vm.Title.Value.Is("potato");
			model.Title.Value = "Controller";
			vm.Title.Value.Is("Controller");
		}
	}
}
