using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	[TestFixture]
	internal class AlbumEditorViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 作成() {
			using var vm = new AlbumEditorViewModel();
			this.Rdb.Albums.Count().Is(0);
			vm.CreateAlbumCommand.Execute();
			vm.SaveCommand.Execute();
			lock (this.Rdb) {
				this.Rdb.Albums.Count().Is(1);
			}
		}

		[Test]
		public void 編集() {
			using var selector = new AlbumSelector("main");
			using var ra = new RegisteredAlbum(selector);
			ra.Create();
			ra.Title.Value = "aa";
			ra.ReflectToDataBase();

			using var vm = new AlbumEditorViewModel();
			vm.EditAlbumCommand.Execute(this.ViewModelFactory.Create(ra));
			vm.Title.Value = "bb";
			ra.Title.Value.Is("aa");

			vm.SaveCommand.Execute();

			ra.Title.Value.Is("bb");
		}
	}
}
