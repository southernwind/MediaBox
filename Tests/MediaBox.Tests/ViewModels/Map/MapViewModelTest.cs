using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.Tests.ViewModels.Map {
	[TestFixture]
	internal class MapViewModelTest : ViewModelTestClassBase {
		[Test]
		public void MapControl() {
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			vm.MapControl.Value.Is(model.MapControl.Value);
		}

		[Test]
		public async Task ItemsForMapView() {
			var image1 = this.MediaFactory.Create(Path.Combine(TestDirectory, "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDirectory, "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDirectory, "image3.jpg"));
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			model.ItemsForMapView.Add(Get.Instance<MediaGroup>(image1, default(Rectangle)));
			model.ItemsForMapView.Add(Get.Instance<MediaGroup>(image2, default(Rectangle)));
			model.ItemsForMapView.Add(Get.Instance<MediaGroup>(image3, default(Rectangle)));

			await Task.Delay(100);

			vm.ItemsForMapView.Count.Is(3);
			vm.ItemsForMapView.Select(x => x.Model).Is(model.ItemsForMapView);
		}

		[Test]
		public void Pointer() {
			var image1 = this.MediaFactory.Create(Path.Combine(TestDirectory, "image1.jpg"));
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			var group = Get.Instance<MediaGroup>(image1, default(Rectangle));
			model.Pointer.Value = group;

			vm.Pointer.Value.Model.Is(group);
		}

		[Test]
		public void PointerLatitude() {
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			model.PointerLatitude.Value = 50.3;
			vm.PointerLatitude.Value.Is(50.3);
		}

		[Test]
		public void PointerLongitude() {
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			model.PointerLongitude.Value = 81.53;
			vm.PointerLongitude.Value.Is(81.53);
		}

		[Test]
		public void BingMapApiKey() {
			var settings = Get.Instance<ISettings>();
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			settings.GeneralSettings.BingMapApiKey.Value = "asdfghjkl";
			vm.BingMapApiKey.Value.Is("asdfghjkl");
		}

		[Test]
		public void MapPinSize() {
			var settings = Get.Instance<ISettings>();
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			settings.GeneralSettings.MapPinSize.Value = 55;
			vm.MapPinSize.Value.Is(55);
		}

		[Test]
		public void ZoomLevel() {
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			model.ZoomLevel.Value = 18;
			vm.ZoomLevel.Value.Is(18);
		}

		[Test]
		public async Task CenterLatitude() {
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			model.CenterLatitude.Value = 5.15;
			await Task.Delay(100);
			vm.CenterLatitude.Value.Is(5.15);
		}

		[Test]
		public async Task CenterLongitude() {
			var model = Get.Instance<MapModel>();
			var vm = Get.Instance<MapViewModel>(model);

			model.CenterLongitude.Value = 2.26;
			await Task.Delay(100);
			vm.CenterLongitude.Value.Is(2.26);
		}

	}
}
