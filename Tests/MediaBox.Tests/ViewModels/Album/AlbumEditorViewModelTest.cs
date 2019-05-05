using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	[TestFixture]
	internal class AlbumEditorViewModelTest : ViewModelTestClassBase {
		[Test]
		public void 作成() {
			var vm = new AlbumEditorViewModel();
			this.DataBase.Albums.Count().Is(0);
			vm.CreateAlbumCommand.Execute();
			vm.SaveCommand.Execute();
			lock (this.DataBase) {
				this.DataBase.Albums.Count().Is(1);
			}
		}

		[Test]
		public void 編集() {
			var ra = new RegisteredAlbum();
			ra.Create();
			ra.Title.Value = "aa";
			ra.AlbumPath.Value = "/pic/test";
			ra.ReflectToDataBase();

			var vm = new AlbumEditorViewModel();
			vm.EditAlbumCommand.Execute(this.ViewModelFactory.Create(ra));
			vm.Title.Value = "bb";
			vm.AlbumPath.Value = "/my/pic";
			ra.Title.Value.Is("aa");
			ra.AlbumPath.Value.Is("/pic/test");

			vm.SaveCommand.Execute();

			ra.Title.Value.Is("bb");
			ra.AlbumPath.Value.Is("/my/pic");
		}

		[Test]
		public void ファイル追加削除() {
			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);

			var r1 = media1.CreateDataBaseRecord();
			var r2 = media2.CreateDataBaseRecord();
			var r3 = media3.CreateDataBaseRecord();
			this.DataBase.MediaFiles.AddRange(r1, r2, r3);
			this.DataBase.SaveChanges();
			media1.MediaFileId = r1.MediaFileId;
			media2.MediaFileId = r2.MediaFileId;
			media3.MediaFileId = r3.MediaFileId;

			var vm = new AlbumEditorViewModel();
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
