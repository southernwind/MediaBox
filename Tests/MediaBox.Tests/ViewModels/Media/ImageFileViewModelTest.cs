using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	internal class ImageFileViewModelTest : ViewModelTestClassBase {
		[Test]
		public void フルサイズイメージ() {
			using var model = new ImageFileModel(this.TestFiles.Image1Jpg.FilePath);
			model.Image = new BitmapImage();
			using var vm = new ImageFileViewModel(model);
			vm.Image.Is(model.Image);
			var args = new List<(object sender, PropertyChangedEventArgs e)>();
			vm.PropertyChanged += (sender, e) => {
				args.Add((sender, e));
			};
			model.Image = new BitmapImage();
			vm.Image.Is(model.Image);
			args.Where(x => x.e.PropertyName == nameof(vm.Image)).Count().Is(1);
		}
	}
}
