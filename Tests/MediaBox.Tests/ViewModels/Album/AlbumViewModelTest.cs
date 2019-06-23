
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	internal class AlbumViewModelTest : ViewModelTestClassBase {
		[Test]
		public void タイトル() {
			var selector = new AlbumSelector("main");
			var model = new RegisteredAlbum(selector);
			model.Title.Value = "title";
			var vm = new AlbumViewModel(model);
			vm.Title.Value.Is("title");
			model.Title.Value = "potato";
			vm.Title.Value.Is("potato");
		}

		[Test]
		public void フィルタリング前件数() {
			var selector = new AlbumSelector("main");
			var model = new RegisteredAlbum(selector);
			model.BeforeFilteringCount.Value = 5;
			var vm = new AlbumViewModel(model);
			vm.BeforeFilteringCount.Value.Is(5);

			model.BeforeFilteringCount.Value = 7;
			vm.BeforeFilteringCount.Value.Is(7);
		}

		[Test]
		public void 表示モード() {
			var selector = new AlbumSelector("main");
			var model = new RegisteredAlbum(selector);
			model.DisplayMode.Value = DisplayMode.Detail;
			var vm = new AlbumViewModel(model);
			vm.DisplayMode.Value.Is(DisplayMode.Detail);

			model.DisplayMode.Value = DisplayMode.Map;
			vm.DisplayMode.Value.Is(DisplayMode.Map);
		}

		[Test]
		public void カレントアイテム() {
			var selector = new AlbumSelector("main");
			var model = new RegisteredAlbum(selector);

			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			model.Items.AddRange(image1, image2, image3, image4);
			var vm = new AlbumViewModel(model);
			model.CurrentIndex.Value = 1;
			vm.CurrentItem.Value.Model.Is(image2);

			model.CurrentMediaFiles.Value = new[] { image4, image3 };
			vm.SelectedMediaFiles.Value.Select(x => x.Model).Is(image4, image3);
		}

		[Test]
		public void 選択アイテム変更() {
			var selector = new AlbumSelector("main");
			var model = new RegisteredAlbum(selector);

			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			model.Items.AddRange(image1, image2, image3, image4);
			var vm = new AlbumViewModel(model);
			vm.SelectedMediaFiles.Value = new[] { this.ViewModelFactory.Create(image2) };
			model.CurrentMediaFiles.Value.Is(image2);

			vm.SelectedMediaFiles.Value = new[] { this.ViewModelFactory.Create(image4), this.ViewModelFactory.Create(image3) };
			model.CurrentMediaFiles.Value.Is(image4, image3);
		}

		[TestCase(DisplayMode.Detail)]
		[TestCase(DisplayMode.Library)]
		[TestCase(DisplayMode.Map)]
		public void 表示モード変更(DisplayMode mode) {
			var selector = new AlbumSelector("main");
			var model = new RegisteredAlbum(selector);
			var vm = new AlbumViewModel(model);

			vm.ChangeDisplayModeCommand.Execute(mode);
			vm.DisplayMode.Value.Is(mode);
			vm.Model.DisplayMode.Value.Is(mode);
		}

		[Test]
		public async Task ファイル追加() {
			var selector = new AlbumSelector("main");
			ReactivePropertyScheduler.SetDefault(UIDispatcherScheduler.Default);
			var (_, image1) = this.Register(this.TestFiles.Image1Jpg);
			var (_, image3) = this.Register(this.TestFiles.Image3Jpg);

			var model = new RegisteredAlbum(selector);
			model.Create();
			var vm = new AlbumViewModel(model);
			vm.AddMediaFileCommand.Execute(new[]{
				this.ViewModelFactory.Create(image1),
				this.ViewModelFactory.Create(image3)
			});

			await RxUtility.WaitPolling(() => model.Items.Count() >= 2, 100, 5000);
			model.Items.Is(image1, image3);
		}

		[Test]
		public async Task フルサイズイメージロード() {
			var selector = new AlbumSelector("main");
			var model = new RegisteredAlbum(selector);

			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			var image4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			var video = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath) as VideoFileModel;
			var image5 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			model.Items.AddRange(image1, image2, image3, image4, video, image5);
			var vm = new AlbumViewModel(model);
			vm.ChangeDisplayModeCommand.Execute(DisplayMode.Detail);

			vm.CurrentIndex.Value = 3;
			await this.WaitTaskCompleted(5000);
			image1.Image.IsNull();
			image2.Image.IsNotNull();
			image3.Image.IsNotNull();
			image4.Image.IsNotNull();
			image5.Image.IsNotNull();

			vm.CurrentIndex.Value = 0;
			await this.WaitTaskCompleted(5000);
			image1.Image.IsNotNull();
			image2.Image.IsNotNull();
			image3.Image.IsNotNull();
			image4.Image.IsNull();
			image5.Image.IsNull();

			vm.CurrentIndex.Value = 5;
			await this.WaitTaskCompleted(5000);
			image1.Image.IsNull();
			image2.Image.IsNull();
			image3.Image.IsNull();
			image4.Image.IsNotNull();
			image5.Image.IsNotNull();
		}

		[Test]
		public async Task モデルリロード() {
			var selector = new AlbumSelector("main");
			// データ準備
			var (r1, _) = this.Register(this.TestFiles.Image1Jpg);
			var (r2, _) = this.Register(this.TestFiles.Image2Jpg);
			var (r3, _) = this.Register(this.TestFiles.Image3Jpg);
			var d = Dispatcher.CurrentDispatcher;

			using var model = new RegisteredAlbum(selector);
			model.Create();
			lock (this.DataBase) {
				this.DataBase.AlbumMediaFiles.AddRange(new[] {
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
				this.DataBase.SaveChanges();
			}
			model.Items.Count.Is(0);

			var vm = new AlbumViewModel(model);
			model.LoadMediaFiles();

			await this.WaitTaskCompleted(3000);
			model.Items.Count.Is(3);

			vm.Items.Count.Is(3);
			vm.Items.Is(model.Items.Select(this.ViewModelFactory.Create));
		}
	}
}
