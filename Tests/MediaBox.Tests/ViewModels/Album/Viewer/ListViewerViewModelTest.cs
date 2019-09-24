
using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Album.Viewer;

namespace SandBeige.MediaBox.Tests.ViewModels.Album.Viewer {
	internal class ListViewerViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			using var selector = new AlbumSelector("main");
			using var model = new RegisteredAlbum(selector);
			using var avm = new AlbumViewModel(model);
			using var vm = new ListViewerViewModel(avm);
		}
	}
}
