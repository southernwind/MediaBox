using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	internal class AlbumBoxViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var selector = new AlbumSelector("main");
			var ra1 = new RegisteredAlbum(selector);
			ra1.AlbumPath.Value = "/picture/sea";
			var ra2 = new RegisteredAlbum(selector);
			ra2.AlbumPath.Value = "/picture/store";
			var ra3 = new RegisteredAlbum(selector);
			ra3.AlbumPath.Value = "";
			var model = new AlbumBox("root", "", new[] { ra1, ra2, ra3 });
			var vm = new AlbumBoxViewModel(model);
			vm.Title.Value.Is("root");
			vm.Children.Select(x => x.Title.Value).Is("picture");
			vm.Albums.Select(x => x.Model).Is(ra3);
		}
	}
}
