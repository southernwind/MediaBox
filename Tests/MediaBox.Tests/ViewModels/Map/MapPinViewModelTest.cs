using System.Windows;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.Tests.ViewModels.Map {
	[TestFixture]
	internal class MapPinViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var rect = new Rectangle(new Point(5, 15), new Size(10, 8));
			var model = new MapPin(image1, rect);
			var vm = new MapPinViewModel(model);
			model.Items.Add(image2);
			model.Items.Add(image3);

			vm.Core.Value.Is(this.ViewModelFactory.Create(image1));
			vm.Count.Value.Is(3);
			vm.Items.Is(
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image2),
				this.ViewModelFactory.Create(image3)
			);
		}
	}
}
