
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	internal class AlbumViewModelTest : ViewModelTestClassBase {
		[Test]
		public void タイトル() {
			using var selector = new AlbumSelector("main");
			using var model = new RegisteredAlbum(selector);
			model.Title.Value = "title";
			using var vm = new AlbumViewModel(model);
			vm.Title.Value.Is("title");
			model.Title.Value = "potato";
			vm.Title.Value.Is("potato");
		}

		[Test]
		public void フィルタリング前件数() {
			using var selector = new AlbumSelector("main");
			using var model = new RegisteredAlbum(selector);
			model.BeforeFilteringCount.Value = 5;
			using var vm = new AlbumViewModel(model);
			vm.BeforeFilteringCount.Value.Is(5);

			model.BeforeFilteringCount.Value = 7;
			vm.BeforeFilteringCount.Value.Is(7);
		}

		[Test]
		public void カレントアイテム() {
			using var selector = new AlbumSelector("main");
			using var model = new RegisteredAlbum(selector);

			using var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			model.Items.AddRange(image1, image2, image3, image4);
			using var vm = new AlbumViewModel(model);
			model.CurrentMediaFile.Value = image2;
			vm.CurrentItem.Value.Model.Is(image2);

			model.CurrentMediaFiles.Value = new[] { image4, image3 };
			vm.SelectedMediaFiles.Value.Select(x => x.Model).Is(image4, image3);
		}

		[Test]
		public void 選択アイテム変更() {
			using var selector = new AlbumSelector("main");
			using var model = new RegisteredAlbum(selector);

			using var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			model.Items.AddRange(image1, image2, image3, image4);
			using var vm = new AlbumViewModel(model);
			vm.SelectedMediaFiles.Value = new[] { this.ViewModelFactory.Create(image2) };
			model.CurrentMediaFiles.Value.Is(image2);

			vm.SelectedMediaFiles.Value = new[] { this.ViewModelFactory.Create(image4), this.ViewModelFactory.Create(image3) };
			model.CurrentMediaFiles.Value.Is(image4, image3);
		}

		[Test]
		public void ファイル追加() {
			using var selector = new AlbumSelector("main");
			ReactivePropertyScheduler.SetDefault(UIDispatcherScheduler.Default);
			var (_, image1) = this.Register(this.TestFiles.Image1Jpg);
			var (_, image3) = this.Register(this.TestFiles.Image3Jpg);

			using var model = new RegisteredAlbum(selector);
			model.Create();
			using var vm = new AlbumViewModel(model);
			vm.AddMediaFileCommand.Execute(new[]{
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image3)
			});

			model.Items.Is(image1, image3);
		}

		[Test]
		public async Task モデルリロード() {
			using var selector = new AlbumSelector("main");
			// データ準備
			var (r1, _) = this.Register(this.TestFiles.Image1Jpg);
			var (r2, _) = this.Register(this.TestFiles.Image2Jpg);
			var (r3, _) = this.Register(this.TestFiles.Image3Jpg);

			using var model = new RegisteredAlbum(selector);
			model.Create();
			lock (this.Rdb) {
				this.Rdb.AlbumMediaFiles.AddRange(new[] {
					new AlbumMediaFile {
						MediaFileId = r1.MediaFileId,
						AlbumId = model.AlbumId.Value
					},new AlbumMediaFile {
						MediaFileId = r2.MediaFileId,
						AlbumId = model.AlbumId.Value
					},new AlbumMediaFile {
						MediaFileId = r3.MediaFileId,
						AlbumId = model.AlbumId.Value
					},
				});
				this.Rdb.SaveChanges();
			}
			model.Items.Count.Is(0);

			using var vm = new AlbumViewModel(model);
			model.LoadMediaFiles();

			await this.WaitTaskCompleted(3000);
			model.Items.Count.Is(3);

			vm.Items.Count.Is(3);
			vm.Items.Is(model.Items.Select(this.ViewModelFactory.Create));
		}
	}
}
