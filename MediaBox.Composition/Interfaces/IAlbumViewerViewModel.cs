using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.ViewModels;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IAlbumViewerViewModel : IViewModelBase {
		IReactiveProperty<bool> IsSelected {
			get;
		}

		IAlbumViewModel AlbumViewModel {
			get;
		}

		IMediaFileListContextMenuViewModel ContextMenuViewModel {
			get;
		}
	}
}
