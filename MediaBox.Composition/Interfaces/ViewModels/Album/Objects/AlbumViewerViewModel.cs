using System;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;

namespace SandBeige.MediaBox.Composition.Interfaces.ViewModels.Album.Objects {
	public abstract class AlbumViewerViewModel : ViewModelBase, IAlbumViewerViewModel {
		public abstract IReactiveProperty<bool> IsSelected {
			get;
		}

		public IAlbumViewModel AlbumViewModel {
			get;
		}

		public IMediaFileListContextMenuViewModel ContextMenuViewModel {
			get;
		}

		protected AlbumViewerViewModel(IAlbumViewModel albumViewModel, IMediaFileListContextMenuViewModel contextMenuViewModel) {
			this.AlbumViewModel = albumViewModel;
			this.ContextMenuViewModel = contextMenuViewModel;

			albumViewModel.SelectedMediaFiles.Subscribe(contextMenuViewModel.SetTargetFiles).AddTo(this.CompositeDisposable);
			albumViewModel.AlbumModel.CurrentAlbumObject.Subscribe(contextMenuViewModel.SetTargetAlbum!).AddTo(this.CompositeDisposable);
		}
	}
}
