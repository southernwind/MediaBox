using System;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interop;

using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Tests.Implements;

namespace SandBeige.MediaBox.Tests.Models.Album.Viewer {
	[Apartment(ApartmentState.STA)]
	internal class TileViewerModelTest : ModelTestClassBase {

		[Test]
		public void キージェスチャーによるカレントアイテム変更() {
			using var selector = new AlbumSelector("main");
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			using var album = new AlbumModelForTest(osc, selector);
			using var model = new TileViewerModel(album);
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
}
