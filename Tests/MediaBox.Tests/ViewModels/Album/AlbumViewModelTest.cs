using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Media;

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

		[Test]
		public async Task MonitoringDirectories() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			vm.MonitoringDirectories.Count.Is(0);
			for (var i = 0; i < 3; i++) {
				model.MonitoringDirectories.Add("a");
			}

			await Task.Delay(30);
			vm.MonitoringDirectories.Count.Is(3);
			for (var i = 0; i < 2; i++) {
				model.MonitoringDirectories.Add("b");
			}
			await Task.Delay(30);
			vm.MonitoringDirectories.Count.Is(5);
			vm.MonitoringDirectories.Is(model.MonitoringDirectories);
		}
	}
}
