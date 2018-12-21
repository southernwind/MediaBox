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
			vm.Count.Value.Is(0);
			vm.Model.Count.Value = 15;
			vm.Count.Value.Is(15);
			vm.Model.Count.Value = int.MaxValue;
			vm.Count.Value.Is(int.MaxValue);
		}

		[Test]
		public async Task Items() {
			var vm = Get.Instance<MediaFileCollectionViewModel<MediaFileCollection>>(Get.Instance<MediaFileCollection>());
			vm.Items.Count.Is(0);
			for (var i = 0; i < 3; i++) {
				vm.Model.Items.Add(Get.Instance<MediaFile>(""));
			}

			await Task.Delay(30);
			vm.Items.Count.Is(3);
			for (var i = 0; i < 2; i++) {
				vm.Model.Items.Add(Get.Instance<MediaFile>(""));
			}
			await Task.Delay(30);
			vm.Items.Count.Is(5);
			vm.Items.Select(x => x.Model).Is(vm.Model.Items);
		}
	}
}
