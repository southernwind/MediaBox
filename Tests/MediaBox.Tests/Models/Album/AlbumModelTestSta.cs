using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;

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

namespace SandBeige.MediaBox.Tests.Models.Album {
	[Apartment(ApartmentState.STA)]
	internal class AlbumModelTestSta : MediaFileCollectionTest {
		/// <summary>
		/// テスト用インスタンス取得
		/// </summary>
		/// <returns>テスト用インスタンス</returns>
		protected override MediaFileCollection GetInstance(ObservableSynchronizedCollection<IMediaFileModel> items, AlbumSelector selector) {
			return new AlbumModelForTest(items, selector);
		}

		[Test]
		public void キージェスチャーによるカレントアイテム変更() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

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
			this.Settings.GeneralSettings.DisplayMode.Value = DisplayMode.Detail;
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

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


		[TestCase(DisplayMode.List)]
		[TestCase(DisplayMode.Map)]
		[TestCase(DisplayMode.Tile)]
		public void DisplayModeDetail以外はマウスホイールによるカレントアイテム変更を行わない(DisplayMode displayMode) {
			this.Settings.GeneralSettings.DisplayMode.Value = displayMode;
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

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

		[Test]
		public void マウスホイールによるズームレベルの変更() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = this.GetInstance(osc, selector) as AlbumModelForTest;

			var gr = (GestureReceiverForTest)album.GestureReceiver;
			gr.IsControlKeyPressed = true;
			this.Settings.GeneralSettings.ZoomLevel.Value = 2;

			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.ZoomLevel.Value.Is(1);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.ZoomLevel.Value.Is(1);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.ZoomLevel.Value.Is(2);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.ZoomLevel.Value.Is(3);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.ZoomLevel.Value.Is(7);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.ZoomLevel.Value.Is(8);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(10));
			album.ZoomLevel.Value.Is(8);
			gr.MouseWheelEventCommand.Execute(this.GetMouseWheelEventArgs(-10));
			album.ZoomLevel.Value.Is(7);

			this.Settings.GeneralSettings.ZoomLevel.Value.Is(7);
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
	}
}
