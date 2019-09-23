using System;
using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class AlbumViewerViewViewModelPairCreator : ModelBase {
		public string Name {
			get;
		}

		public Func<UserControl> ViewerCreator {
			get;
		}

		public Func<IAlbumViewModel, IAlbumViewerViewModel> ViewModelCreator {
			get;
		}

		public AlbumViewerViewViewModelPairCreator(string name, Func<UserControl> viewerCreator, Func<IAlbumViewModel, IAlbumViewerViewModel> viewModelCreator) {
			this.Name = name;
			this.ViewerCreator = viewerCreator;
			this.ViewModelCreator = viewModelCreator;
		}

		public IAlbumViewerViewViewModelPair Create(IAlbumViewModel album) {
			return new AlbumViewerViewViewModelPair(this.Name, this.ViewerCreator(), this.ViewModelCreator(album));
		}
	}
}
