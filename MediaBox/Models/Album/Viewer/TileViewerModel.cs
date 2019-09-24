
using System;
using System.Windows.Input;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class TileViewerModel : ModelBase {

		/// <summary>
		/// 表示する列
		/// </summary>
		public ReadOnlyReactiveCollection<Col> Columns {
			get;
		}


		public IReactiveProperty<bool> IsVisible {
			get;
		} = new ReactivePropertySlim<bool>();


		public TileViewerModel(IAlbumModel albumModel) {
			albumModel.GestureReceiver
				.KeyEvent
				.Subscribe(x => {
					if (!this.IsVisible.Value) {
						return;
					}
					switch (x.Key) {
						case Key.Left:
							if (x.IsDown) {
								albumModel.SelectPreviewItem();
							}
							break;
						case Key.Right:
							if (x.IsDown) {
								albumModel.SelectNextItem();
							}
							break;

					}
				}).AddTo(this.CompositeDisposable);
		}
	}
}
