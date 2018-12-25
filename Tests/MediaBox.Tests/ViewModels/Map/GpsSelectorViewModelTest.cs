using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Map {
	[TestFixture]
	internal class GpsSelectorViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Latitude() {
			var model = Get.Instance<GpsSelector>();
			var vm = Get.Instance<GpsSelectorViewModel>(model);
			model.Latitude.Value = 55;
			vm.Latitude.Value.Is(55);
		}

		[Test]
		public void Longitude() {
			var model = Get.Instance<GpsSelector>();
			var vm = Get.Instance<GpsSelectorViewModel>(model);
			model.Longitude.Value = 83.4;
			vm.Longitude.Value.Is(83.4);
		}

		[Test]
		public void Map() {
			var model = Get.Instance<GpsSelector>();
			var vm = Get.Instance<GpsSelectorViewModel>(model);

			vm.Map.Value.MapControl.Value.Is(model.Map.Value.MapControl.Value);
		}

		[Test]
		public void TargetFiles() {
			var image1 = Get.Instance<MediaFile>(Path.Combine(TestDirectory, "image1.jpg"));
			var image2 = Get.Instance<MediaFile>(Path.Combine(TestDirectory, "image2.jpg"));
			var image3 = Get.Instance<MediaFile>(Path.Combine(TestDirectory, "image3.jpg"));
			var model = Get.Instance<GpsSelector>();
			var vm = Get.Instance<GpsSelectorViewModel>(model);

			vm.TargetFiles.Add(Get.Instance<MediaFileViewModel>(image1));
			vm.TargetFiles.Add(Get.Instance<MediaFileViewModel>(image2));
			vm.TargetFiles.Add(Get.Instance<MediaFileViewModel>(image3));

			vm.TargetFiles.Select(x => x.Model).Is(model.TargetFiles);
		}


		[Test]
		public async Task SetCandidateMediaFiles() {
			var model = Get.Instance<GpsSelector>();
			var vm = Get.Instance<GpsSelectorViewModel>(model);
			var images = new[] {
				Get.Instance<MediaFile>(Path.Combine(TestDirectory, "image1.jpg")),
				Get.Instance<MediaFile>(Path.Combine(TestDirectory, "image2.jpg")),
				Get.Instance<MediaFile>(Path.Combine(TestDirectory, "image3.jpg"))
			};

			vm.SetCandidateMediaFiles(images.Select(x => Get.Instance<MediaFileViewModel>(x)));
			await Task.Delay(100);
			vm.CandidateMediaFiles.Count.Is(3);
			vm.CandidateMediaFiles.Select(x => x.Model).Is(model.CandidateMediaFiles);
		}
	}
}
