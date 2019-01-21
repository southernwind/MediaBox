using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.Map;

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
			var image1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
			var model = Get.Instance<GpsSelector>();
			var vm = Get.Instance<GpsSelectorViewModel>(model);
			vm.TargetFiles.Value = new[]{
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image2),
				this.ViewModelFactory.Create(image3)
			};

			vm.TargetFiles.Value.Select(x => x.Model).Is(model.TargetFiles.Value);
		}


		[Test]
		public async Task SetCandidateMediaFiles() {
			var model = Get.Instance<GpsSelector>();
			var vm = Get.Instance<GpsSelectorViewModel>(model);
			var images = new[] {
				this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg")),
				this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg")),
				this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"))
			};

			vm.SetCandidateMediaFiles(images.Select(this.ViewModelFactory.Create));
			await Task.Delay(100);
			vm.CandidateMediaFiles.Count.Is(3);
			vm.CandidateMediaFiles.Select(x => x.Model).Is(model.CandidateMediaFiles);
		}
	}
}
