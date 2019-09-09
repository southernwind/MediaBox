using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.Tests.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	internal class AlbumTest : MediaFileCollectionTest {
		/// <summary>
		/// テスト用インスタンス取得
		/// </summary>
		/// <returns>テスト用インスタンス</returns>
		protected override MediaFileCollection GetInstance(ObservableSynchronizedCollection<IMediaFileModel> items, AlbumSelector selector) {
			return new AlbumModelForTest(items, selector);
		}

		[Test]
		public void カレントアイテム変更() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var media4 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			using var media5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);

			album.Items.AddRange(media1, media2, media3, media4, media5);

			album.CurrentMediaFile.Value.IsNull();
			album.MediaFileInformation.Value.Files.Value.Is();

			album.CurrentMediaFiles.Value = new[] { media3, media1, media5 };
			album.CurrentMediaFile.Value.Is(media3);
			album.MediaFileInformation.Value.Files.Value.Is(media3, media1, media5);

			album.CurrentMediaFiles.Value = new[] { media5 };
			album.CurrentMediaFile.Value.Is(media5);
			album.MediaFileInformation.Value.Files.Value.Is(media5);

			album.CurrentMediaFiles.Value = Array.Empty<IMediaFileModel>();
			album.CurrentMediaFile.Value.IsNull();
			album.MediaFileInformation.Value.Files.Value.Is();

			album.CurrentMediaFiles.Value = new[] { media1, media4 };
			album.CurrentMediaFile.Value.Is(media1);
			album.MediaFileInformation.Value.Files.Value.Is(media1, media4);
		}

		[Test]
		public void Mapメディアファイルコレクション() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;
			(album.Map.Value.Items == album.Items).IsTrue();
		}

		[Test]
		public void Mapカレントアイテム() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var media4 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			using var media5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);

			album.Items.AddRange(media1, media2, media3, media4, media5);

			album.Map.Value.CurrentMediaFiles.Value.Is();

			album.CurrentMediaFiles.Value = new[] { media2, media4, media5 };
			album.Map.Value.CurrentMediaFiles.Value.Is(media2, media4, media5);

			album.CurrentMediaFiles.Value = Array.Empty<IMediaFileModel>();
			album.Map.Value.CurrentMediaFiles.Value.Is();
		}

		[Test]
		public async Task メディアリストロード() {
			using var selector = new AlbumSelector("main");
			// データ準備
			this.Register(this.TestFiles.Image1Jpg);
			this.Register(this.TestFiles.Image2Jpg);
			this.Register(this.TestFiles.Image3Jpg);

			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;
			album.Items.Count.Is(0);
			album.LoadMediaFiles();
			await this.WaitTaskCompleted(3000);
			album.Items.Count.Is(3);
			album.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);

			album.Predicate = _ => false;
			album.LoadMediaFiles();
			await this.WaitTaskCompleted(3000);
			album.Items.Count.Is(0);
			album.Items.Check();

			// Image1,Image2はHeight=5で、Image3だけHeight=4
			album.Predicate = m => m.Height == 5;
			album.LoadMediaFiles();
			await this.WaitTaskCompleted(3000);
			album.Items.Count.Is(2);
			album.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg);
		}

		[Test]
		public async Task ファイル削除追従() {
			using var selector = new AlbumSelector("main");
			using var mfm = Get.Instance<MediaFileManager>();

			// データ準備
			var (_, m1) = this.Register(this.TestFiles.Image1Jpg);
			var (_, m2) = this.Register(this.TestFiles.Image2Jpg);
			this.Register(this.TestFiles.Image3Jpg);
			var (_, m4) = this.Register(this.TestFiles.Image4Png);
			this.Register(this.TestFiles.Image5Bmp);

			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;
			album.Items.Count.Is(0);
			album.LoadMediaFiles();
			await this.WaitTaskCompleted(3000);
			album.Items.Count.Is(5);
			album.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg, this.TestFiles.Image4Png, this.TestFiles.Image5Bmp);

			mfm.DeleteItems(new[] { m4, m2 });

			album.Items.Count.Is(3);
			album.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image3Jpg, this.TestFiles.Image5Bmp);

			mfm.DeleteItems(new[] { m1 });

			album.Items.Count.Is(2);
			album.Items.Check(this.TestFiles.Image3Jpg, this.TestFiles.Image5Bmp);
		}

		[TestCase(DisplayMode.Detail)]
		[TestCase(DisplayMode.List)]
		[TestCase(DisplayMode.Tile)]
		[TestCase(DisplayMode.Map)]
		public void DisplayMode変更(DisplayMode mode) {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;
			album.ChangeDisplayMode(mode);
			album.DisplayMode.Value.Is(mode);
			this.Settings.GeneralSettings.DisplayMode.Value.Is(mode);
		}

		[Test]
		public async Task フルサイズ事前読み込み() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath) as VideoFileModel;

			album.Items.AddRange(media1, media2, media3, media4, media5);

			album.Prefetch(new[] { media1 });
			media1.Image.IsNull();
			await this.WaitTaskCompleted(3000);
			media1.Image.IsInstanceOf<ImageSource>();

			album.Prefetch(new[] { media2 });
			media1.Image.IsNull();
			media2.Image.IsNull();
			await this.WaitTaskCompleted(3000);
			media2.Image.IsInstanceOf<ImageSource>();
		}

		[Test]
		public async Task フルサイズ事前読み込み順() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath) as VideoFileModel;

			album.Items.AddRange(media1, media2, media3, media4, media5);


			album.Prefetch(new[] { media3, media1, media4 });
			var loaded = new List<int>();
			media1.PropertyChanged += (sender, _) => {
				loaded.Add(1);
			};
			media3.PropertyChanged += (sender, _) => {
				loaded.Add(3);
			};
			media4.PropertyChanged += (sender, _) => {
				loaded.Add(4);
			};
			await this.WaitTaskCompleted(3000);
			loaded.Is(3, 1, 4);
		}

		[Test]
		public async Task カレントファイル変更を契機としたフルサイズ読み込み() {
			this.Settings.GeneralSettings.DisplayMode.Value = DisplayMode.Detail;
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Image5Bmp.FilePath) as ImageFileModel;
			using var media6 = this.MediaFactory.Create(this.TestFiles.Image6Gif.FilePath) as ImageFileModel;
			using var media7 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			using var media8 = this.MediaFactory.Create(this.TestFiles.SpecialFileNameImageJpg.FilePath) as ImageFileModel;

			var models = new[] { media1, media2, media3, media4, media5, media6, media7, media8 };
			album.Items.AddRange(models);

			album.CurrentMediaFiles.Value = new[] { media1, media5, media6 };
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is(media1, media2, media3);

			album.CurrentMediaFiles.Value = new[] { media8, media5 };
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is(media6, media7, media8);

			album.CurrentMediaFiles.Value = new[] { media4 };
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is(media2, media3, media4, media5, media6);
		}

		[TestCase(DisplayMode.List)]
		[TestCase(DisplayMode.Map)]
		[TestCase(DisplayMode.Tile)]
		public async Task DisplayModeDetail以外はフルサイズ読み込みを行わない(DisplayMode displayMode) {
			this.Settings.GeneralSettings.DisplayMode.Value = displayMode;
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Image5Bmp.FilePath) as ImageFileModel;
			using var media6 = this.MediaFactory.Create(this.TestFiles.Image6Gif.FilePath) as ImageFileModel;
			using var media7 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			using var media8 = this.MediaFactory.Create(this.TestFiles.SpecialFileNameImageJpg.FilePath) as ImageFileModel;

			var models = new[] { media1, media2, media3, media4, media5, media6, media7, media8 };
			album.Items.AddRange(models);

			album.CurrentMediaFiles.Value = new[] { media1, media5, media6 };
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is();
		}

		[Test]
		public async Task DisplayMode変更を契機としたフルサイズ読み込み() {
			this.Settings.GeneralSettings.DisplayMode.Value = DisplayMode.List;
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Image5Bmp.FilePath) as ImageFileModel;
			using var media6 = this.MediaFactory.Create(this.TestFiles.Image6Gif.FilePath) as ImageFileModel;
			using var media7 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			using var media8 = this.MediaFactory.Create(this.TestFiles.SpecialFileNameImageJpg.FilePath) as ImageFileModel;

			var models = new[] { media1, media2, media3, media4, media5, media6, media7, media8 };
			album.Items.AddRange(models);

			album.CurrentMediaFiles.Value = new[] { media1, media5, media6 };
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is();

			album.DisplayMode.Value = DisplayMode.Map;
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is();

			album.DisplayMode.Value = DisplayMode.Detail;
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is(media1, media2, media3);
		}

		[Test]
		public async Task DisplayModeをDetail以外に変更した場合のフルサイズイメージアンロード() {
			this.Settings.GeneralSettings.DisplayMode.Value = DisplayMode.Detail;
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Image5Bmp.FilePath) as ImageFileModel;
			using var media6 = this.MediaFactory.Create(this.TestFiles.Image6Gif.FilePath) as ImageFileModel;
			using var media7 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath) as ImageFileModel;
			using var media8 = this.MediaFactory.Create(this.TestFiles.SpecialFileNameImageJpg.FilePath) as ImageFileModel;

			var models = new[] { media1, media2, media3, media4, media5, media6, media7, media8 };
			album.Items.AddRange(models);

			album.CurrentMediaFiles.Value = new[] { media1, media5, media6 };
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is(media1, media2, media3);

			album.DisplayMode.Value = DisplayMode.Tile;
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is();
		}
	}
}
