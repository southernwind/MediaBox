using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaFileCollectionViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Count() {
			var vm = Get.Instance<MediaFileCollectionViewModel<MediaFileCollection>>(Get.Instance<MediaFileCollection>());
			Assert.AreEqual(0, vm.Count.Value);
			vm.Model.Count.Value = 15;
			Assert.AreEqual(15, vm.Count.Value);
			vm.Model.Count.Value = int.MaxValue;
			Assert.AreEqual(int.MaxValue, vm.Count.Value);
		}

		[Test]
		public async Task Items() {
			var vm = Get.Instance<MediaFileCollectionViewModel<MediaFileCollection>>(Get.Instance<MediaFileCollection>());
			Assert.AreEqual(0, vm.Items.Count);
			for (var i = 0; i < 3; i++) {
				vm.Model.Items.Add(Get.Instance<MediaFile>(""));
			}

			await Task.Delay(30);
			Assert.AreEqual(3, vm.Items.Count);
			for (var i = 0; i < 2; i++) {
				vm.Model.Items.Add(Get.Instance<MediaFile>(""));
			}
			await Task.Delay(30);
			Assert.AreEqual(5, vm.Items.Count);
			CollectionAssert.AreEqual(vm.Model.Items, vm.Items.Select(x => x.Model));
		}
	}
}
