using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.Tests.ViewModels.Map {
	[TestFixture]
	internal class GpsSelectorViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 座標() {
			var model = new GpsSelector();
			var vm = new GpsSelectorViewModel(model);
			model.Location.Value = new GpsLocation(55, 88);
			vm.Location.Value.Latitude.Is(55);
			vm.Location.Value.Longitude.Is(88);
		}

		[Test]
		public void マップコントロール() {
			var model = new GpsSelector();
			var vm = new GpsSelectorViewModel(model);

			vm.Map.Value.MapControl.Value.Is(model.Map.Value.MapControl.Value);
		}

		[Test]
		public void 処理対象ファイル() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var model = new GpsSelector();
			var vm = new GpsSelectorViewModel(model);
			vm.TargetFiles.Value = new[]{
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image2),
				this.ViewModelFactory.Create(image3)
			};

			vm.TargetFiles.Value.Select(x => x.Model).Is(model.TargetFiles.Value);
		}


		[Test]
		public void Gps選択設定候補リスト() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var model = new GpsSelector();
			var vm = new GpsSelectorViewModel(model);

			vm.SetCandidateMediaFiles(new[]{
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image2),
				this.ViewModelFactory.Create(image3)
			});
			vm.CandidateMediaFiles.Count.Is(3);
			vm.CandidateMediaFiles.Select(x => x.Model).Is(model.CandidateMediaFiles);
		}
	}
}
