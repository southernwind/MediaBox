using System;
using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class AlbumViewerCreator : ModelBase {
		public string Name {
			get;
		}

		public Func<UserControl> ViewerCreator {
			get;
		}

		public Func<IAlbumViewModel, IAlbumViewerViewModel> ViewModelCreator {
			get;
		}

		public AlbumViewerCreator(string name, Func<UserControl> viewerCreator, Func<IAlbumViewModel, IAlbumViewerViewModel> viewModelCreator) {
			this.Name = name;
			this.ViewerCreator = viewerCreator;
			this.ViewModelCreator = viewModelCreator;
		}

		public IAlbumViewer Create(IAlbumViewModel album) {
			return new AlbumViewer(this.Name, this.ViewerCreator(), this.ViewModelCreator(album));
		}
	}
}
