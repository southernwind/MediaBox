
using System.Linq;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	internal class AlbumBoxViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			using var selector = new AlbumSelector("main");
			using var ra1 = new RegisteredAlbum(selector);
			using var ra2 = new RegisteredAlbum(selector);
			using var ra3 = new RegisteredAlbum(selector);
			using var rc = new ReactiveCollection<RegisteredAlbum>();
			using var rorc = rc.ToReadOnlyReactiveCollection();
			using var model = new AlbumBox(rorc);
			using var vm = new AlbumBoxViewModel(model);

			rc.AddRange(ra1, ra2, ra3);
			vm.Albums.Select(x => x.Model).Is(ra1, ra2, ra3);
		}
	}
}
