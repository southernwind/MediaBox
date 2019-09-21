
using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	internal class ListViewModel : ViewModelBase {
		public IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public IAlbumViewModel AlbumViewModel {
			get;
		}
		public ListViewModel(IAlbumViewModel albumViewModel) {
			this.AlbumViewModel = albumViewModel;
		}
	}
}
