using System.Windows.Controls;

using Livet;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class AlbumViewer : ModelBase, IAlbumViewer {
		public string Name {
			get;
		}

		public UserControl Viewer {
			get;
		}

		public ViewModel ViewModel {
			get;
		}

		public AlbumViewer(string name, UserControl viewer, ViewModel viewModel) {
			this.Name = name;
			this.Viewer = viewer;
			this.ViewModel = viewModel;
		}
	}
}
