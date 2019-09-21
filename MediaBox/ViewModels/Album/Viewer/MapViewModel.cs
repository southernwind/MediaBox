using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	internal class MapViewModel : ViewModelBase {
		public IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public IAlbumViewModel AlbumViewModel {
			get;
		}
		public MapViewModel(IAlbumViewModel albumViewModel) {
			this.AlbumViewModel = albumViewModel;

		}
	}
}
