using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Album.Viewer;

namespace SandBeige.MediaBox.Tests.ViewModels.Album.Viewer {
	internal class DetailViewerViewModelTest : ViewModelTestClassBase {
		[Test]
		public async Task フルサイズイメージロード() {
			using var selector = new AlbumSelector("main");
			using var model = new RegisteredAlbum(selector);

			using var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var video = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath) as VideoFileModel;
			using var image5 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			model.Items.AddRange(image1, image2, image3, image4, video, image5);
			using var avm = new AlbumViewModel(model);
			using var vm = new DetailViewerViewModel(avm);
			vm.IsSelected.Value = true;

			model.CurrentMediaFiles.Value = new[] { image4 };
			await this.WaitTaskCompleted(5000);
			image1.Image.IsNull();
			image2.Image.IsNotNull();
			image3.Image.IsNotNull();
			image4.Image.IsNotNull();
			image5.Image.IsNotNull();

			model.CurrentMediaFiles.Value = new[] { image1 };
			await this.WaitTaskCompleted(5000);
			image1.Image.IsNotNull();
			image2.Image.IsNotNull();
			image3.Image.IsNotNull();
			image4.Image.IsNull();
			image5.Image.IsNull();

			model.CurrentMediaFiles.Value = new[] { image5 };
			await this.WaitTaskCompleted(5000);
			image1.Image.IsNull();
			image2.Image.IsNull();
			image3.Image.IsNull();
			image4.Image.IsNotNull();
			image5.Image.IsNotNull();
		}
	}
}
