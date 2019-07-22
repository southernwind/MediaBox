using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	internal class VideoFileViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 回転() {
			using var model = new VideoFileModel(this.TestFiles.Image1Jpg.FilePath);
			model.Rotation = 15;
			using var vm = new VideoFileViewModel(model);
			vm.Rotation.Is(15);
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.Rotation = 90;
			vm.Rotation.Is(90);
			args.Count(x => x.e.PropertyName == nameof(vm.Rotation)).Is(1);
		}
	}
}
