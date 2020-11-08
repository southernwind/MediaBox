
using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.Album.Objects;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;

namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle {
	internal class ViewerViewModel : AlbumViewerViewModel {
		public override IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public ViewerViewModel(IAlbumViewModel albumViewModel, IMediaFileListContextMenuViewModel contextMenuViewModel) : base(albumViewModel, contextMenuViewModel) {
		}
	}
}
