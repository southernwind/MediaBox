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
			this.DataBase.Albums.Count().Is(0);
			vm.CreateAlbumCommand.Execute();
			vm.SaveCommand.Execute();
			lock (this.DataBase) {
				this.DataBase.Albums.Count().Is(1);
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

		[Test]
		public void ファイル追加削除() {
			var (_, media1) = this.Register(this.TestFiles.Image1Jpg);
			var (_, media2) = this.Register(this.TestFiles.Image2Jpg);
			var (_, media3) = this.Register(this.TestFiles.Image3Jpg);

			using var vm = new AlbumEditorViewModel();
			this.DataBase.Albums.Count().Is(0);
			vm.CreateAlbumCommand.Execute();
			vm.SelectedNotAddedMediaFiles.Value = new[]{
				this.ViewModelFactory.Create(media1),
				this.ViewModelFactory.Create(media2),
				this.ViewModelFactory.Create(media3)
			};
			vm.Items.Is();
			vm.AddFilesCommand.Execute();
			vm.Items.Is(
				this.ViewModelFactory.Create(media1),
				this.ViewModelFactory.Create(media2),
				this.ViewModelFactory.Create(media3)
			);
			vm.SelectedNotAddedMediaFiles.Value.Is();

			vm.SelectedAddedMediaFiles.Value = new[] { this.ViewModelFactory.Create(media2) };
			vm.RemoveFilesCommand.Execute();
			vm.Items.Is(
				this.ViewModelFactory.Create(media1),
				this.ViewModelFactory.Create(media3)
			);
		}
	}
}
