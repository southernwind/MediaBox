
using Prism.Mvvm;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle {
	internal class ViewerViewModel : BindableBase, IAlbumViewerViewModel {
		public IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public IAlbumViewModel AlbumViewModel {
			get;
		}

		public ViewerViewModel(IAlbumViewModel albumViewModel) {
			this.AlbumViewModel = albumViewModel;
		}
	}
}
