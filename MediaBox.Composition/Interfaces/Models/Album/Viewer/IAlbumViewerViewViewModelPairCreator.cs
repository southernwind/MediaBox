using System;
using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Viewer {
	public interface IAlbumViewerViewViewModelPairCreator : IModelBase {
		string Name {
			get;
		}
		Func<UserControl> ViewerCreator {
			get;
		}
		Func<IAlbumViewModel, IMediaFileListContextMenuViewModel, IAlbumViewerViewModel> ViewModelCreator {
			get;
		}

		IAlbumViewerViewViewModelPair Create(IAlbumViewModel album, IMediaFileListContextMenuViewModel contextMenuViewModel);
	}
}