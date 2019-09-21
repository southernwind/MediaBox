using System;
using System.Windows.Input;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	internal class TileViewModel : ViewModelBase {
		public IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public IAlbumViewModel AlbumViewModel {
			get;
		}
		public TileViewModel(IAlbumViewModel albumViewModel) {
			var albumModel = (albumViewModel as AlbumViewModel).Model;
			this.AlbumViewModel = albumViewModel;

			albumModel.GestureReceiver
				.KeyEvent
				.Subscribe(x => {
					if (!this.IsSelected.Value) {
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
