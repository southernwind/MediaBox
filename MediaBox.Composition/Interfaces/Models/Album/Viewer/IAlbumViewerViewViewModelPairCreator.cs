using System;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Viewer {
	public interface IAlbumViewerViewViewModelPairCreator : IModelBase {
		string Name {
			get;
		}
		Func<UserControl> ViewerCreator {
			get;
		}
		Func<IAlbumViewModel, IAlbumViewerViewModel> ViewModelCreator {
			get;
		}

		IAlbumViewerViewViewModelPair Create(IAlbumViewModel album);
	}
}