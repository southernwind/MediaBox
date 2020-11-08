
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.Album.Objects;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;
using SandBeige.MediaBox.Models.Album.Viewer;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	public class DetailViewerViewModel : AlbumViewerViewModel {
		public override IReactiveProperty<bool> IsSelected {
			get;
		}

		public DetailViewerViewModel(IAlbumViewModel albumViewModel, IMediaFileListContextMenuViewModel contextMenuViewModel) : base(albumViewModel, contextMenuViewModel) {
			var model = new DetailViewerModel(this.AlbumViewModel.AlbumModel);

			this.IsSelected = model.IsVisible.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
		}
	}
}
