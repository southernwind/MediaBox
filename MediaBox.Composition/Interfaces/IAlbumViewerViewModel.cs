using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IAlbumViewerViewModel {
		IReactiveProperty<bool> IsSelected {
			get;
		}

		IAlbumViewModel AlbumViewModel {
			get;
		}
	}
}
