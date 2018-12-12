using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	[TestFixture]
	internal class AlbumViewModelTest :ViewModelTestClassBase{
		[Test]
		public void Title() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			Assert.IsNull(vm.Title.Value);
			model.Title.Value = "potato";
			Assert.AreEqual("potato", vm.Title.Value);
			model.Title.Value = "Controller";
			Assert.AreEqual("Controller", vm.Title.Value);
		}

		[Test]
		public async Task MonitoringDirectories() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			Assert.AreEqual(0, vm.MonitoringDirectories.Count);
			for (var i = 0; i < 3; i++) {
				model.MonitoringDirectories.Add("a");
			}

			await Task.Delay(30);
			Assert.AreEqual(3, vm.MonitoringDirectories.Count);
			for (var i = 0; i < 2; i++) {
				model.MonitoringDirectories.Add("b");
			}
			await Task.Delay(30);
			Assert.AreEqual(5, vm.MonitoringDirectories.Count);
			CollectionAssert.AreEqual(model.MonitoringDirectories, vm.MonitoringDirectories);
		}

		[Test]
		public async Task SelectedMediaFiles() {
			var model = Get.Instance<RegisteredAlbum>();
			var vm = Get.Instance<AlbumViewModel>(model);
			Assert.AreEqual(0, vm.SelectedMediaFiles.Count);
			Assert.AreEqual(0, vm.MediaFilePropertiesViewModel.Value.Count.Value);
			for (var i = 0; i < 3; i++) {
				vm.SelectedMediaFiles.Add(Get.Instance<MediaFileViewModel>(Get.Instance<MediaFile>("")));
			}

			await Task.Delay(30);
			Assert.AreEqual(3, vm.MediaFilePropertiesViewModel.Value.Count.Value);
			for (var i = 0; i < 2; i++) {
				vm.SelectedMediaFiles.Add(Get.Instance<MediaFileViewModel>(Get.Instance<MediaFile>("")));
			}
			await Task.Delay(30);
			Assert.AreEqual(5, vm.MediaFilePropertiesViewModel.Value.Count.Value);
			CollectionAssert.AreEqual(vm.MediaFilePropertiesViewModel.Value.Items.Select(x => x.Model), vm.SelectedMediaFiles.Select(x => x.Model));
		}
	}
}
