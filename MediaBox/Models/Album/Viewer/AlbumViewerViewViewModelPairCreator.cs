using System;
using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Viewer;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	public class AlbumViewerViewViewModelPairCreator : ModelBase, IAlbumViewerViewViewModelPairCreator {
		public string Name {
			get;
		}

		public Func<UserControl> ViewerCreator {
			get;
		}

		public Func<IAlbumViewModel, IMediaFileListContextMenuViewModel, IAlbumViewerViewModel> ViewModelCreator {
			get;
		}

		public AlbumViewerViewViewModelPairCreator(string name, Func<UserControl> viewerCreator, Func<IAlbumViewModel, IMediaFileListContextMenuViewModel, IAlbumViewerViewModel> viewModelCreator) {
			this.Name = name;
			this.ViewerCreator = viewerCreator;
			this.ViewModelCreator = viewModelCreator;
		}

		public IAlbumViewerViewViewModelPair Create(IAlbumViewModel album, IMediaFileListContextMenuViewModel contextMenuViewModel) {
			return new AlbumViewerViewViewModelPair(this.Name, this.ViewerCreator(), this.ViewModelCreator(album, contextMenuViewModel));
		}
	}
}
