using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

using Livet.StatefulModel;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Tests.Implements;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Viewer {
	[Apartment(ApartmentState.STA)]
	internal class DetailViewerModelTestSta : ModelTestClassBase {
		[Test]
		public void キージェスチャーによるカレントアイテム変更() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new DetailViewerModel(album);
			model.IsVisible.Value = true;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Image5Bmp.FilePath) as ImageFileModel;

			var models = new[] { media1, media2, media3, media4, media5 };
			album.Items.AddRange(models);

			var gr = (GestureReceiverForTest)album.GestureReceiver;
			album.CurrentMediaFiles.Value = new[] { media1, media3, media5 };

			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Right));
			album.CurrentMediaFiles.Value.Is(media2);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Left));
			album.CurrentMediaFiles.Value.Is(media1);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Left));
			album.CurrentMediaFiles.Value.Is(media1);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Right));
			album.CurrentMediaFiles.Value.Is(media2);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Right));
			album.CurrentMediaFiles.Value.Is(media3);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Right));
			album.CurrentMediaFiles.Value.Is(media4);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Right));
			album.CurrentMediaFiles.Value.Is(media5);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Right));
			album.CurrentMediaFiles.Value.Is(media5);
			gr.KeyEventCommand.Execute(this.GetKeyEventArgs(Key.Left));
			album.CurrentMediaFiles.Value.Is(media4);
		}

		[Test]
		public void マウスジェスチャーによるカレントアイテム変更() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new DetailViewerModel(album);
			model.IsVisible.Value = true;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Image5Bmp.FilePath) as ImageFileModel;

			var models = new[] { media1, media2, media3, media4, media5 };
			album.Items.AddRange(models);

			var gr = (GestureReceiverForTest)album.GestureReceiver;
			gr.IsControlKeyPressed = false;
			album.CurrentMediaFiles.Value = new[] { media1, media3, media5 };

			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.CurrentMediaFiles.Value.Is(media2);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.CurrentMediaFiles.Value.Is(media1);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.CurrentMediaFiles.Value.Is(media1);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.CurrentMediaFiles.Value.Is(media2);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.CurrentMediaFiles.Value.Is(media3);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.CurrentMediaFiles.Value.Is(media4);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.CurrentMediaFiles.Value.Is(media5);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.CurrentMediaFiles.Value.Is(media5);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.CurrentMediaFiles.Value.Is(media4);

			gr.IsControlKeyPressed = true;
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.CurrentMediaFiles.Value.Is(media4);
		}

		[Test]
		public void DetailViewer表示中以外はマウスホイールによるカレントアイテム変更を行わない() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new DetailViewerModel(album);
			model.IsVisible.Value = false;

			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath) as ImageFileModel;
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath) as ImageFileModel;
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath) as ImageFileModel;
			using var media5 = this.MediaFactory.Create(this.TestFiles.Image5Bmp.FilePath) as ImageFileModel;

			var models = new[] { media1, media2, media3, media4, media5 };
			album.Items.AddRange(models);

			var gr = (GestureReceiverForTest)album.GestureReceiver;
			gr.IsControlKeyPressed = false;
			album.CurrentMediaFiles.Value = new[] { media1, media3, media5 };

			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.CurrentMediaFiles.Value.Is(media1, media3, media5);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.CurrentMediaFiles.Value.Is(media1, media3, media5);
		}

		/// <summary>
		/// Delta値を指定して<see cref="MouseWheelEventArgs"/>のインスタンスを生成します。
		/// </summary>
		/// <param name="delta">Delta値</param>
		/// <returns><see cref="MouseWheelEventArgs"/>インスタンス</returns>
		private MouseWheelEventArgs GetMouseWheelEventArgs(int delta) {
			var ea = new MouseWheelEventArgs(ObjectCreator.MouseDevice(), 0, delta) {
				RoutedEvent = ObjectCreator.RoutedEvent()
			};
			return ea;
		}

		/// <summary>
		/// キーを指定して<see cref="KeyEventArgs"/>のインスタンスを生成します。
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns><see cref="KeyEventArgs"/>インスタンス</returns>
		private KeyEventArgs GetKeyEventArgs(Key key) {
			var kbd = new KeyboardDeviceForTest(InputManager.Current) {
				DownKeys = new[] { key }
			};
			return new KeyEventArgs(kbd, new HwndSource(0, 0, 0, 0, 0, "name", IntPtr.Zero), 0, key);
		}
	}

	internal class DetailViewerModelTest : ModelTestClassBase {

		[Test]
		public async Task カレントファイル変更を契機としたフルサイズ読み込み() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new DetailViewerModel(album);
			model.IsVisible.Value = true;

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

		[Test]
		public async Task DetailViewer表示中以外はカレントファイル変更してもフルサイズ読み込みを行わない() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new DetailViewerModel(album);
			model.IsVisible.Value = false;

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
		public async Task DetailViewer表示切り替えを契機としたフルサイズ読み込み() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new DetailViewerModel(album);
			model.IsVisible.Value = false;

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

			model.IsVisible.Value = true;
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is(media1, media2, media3);
		}

		[Test]
		public async Task DetailViewer表示中から非表示に変更した場合のフルサイズイメージアンロード() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new DetailViewerModel(album);
			model.IsVisible.Value = true;

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

			model.IsVisible.Value = false;
			await this.WaitTaskCompleted(3000);
			models.Where(x => x.Image is ImageSource).Is();
		}
	}
}
