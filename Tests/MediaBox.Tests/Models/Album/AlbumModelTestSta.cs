using System.Threading;
using System.Windows.Input;

using Livet.StatefulModel;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
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
