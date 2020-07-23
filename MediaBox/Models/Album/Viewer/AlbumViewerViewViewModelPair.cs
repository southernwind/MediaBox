using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	public class AlbumViewerViewViewModelPair : ModelBase, IAlbumViewerViewViewModelPair {
		public string Name {
			get;
		}

		public UserControl View {
			get;
		}

		public IAlbumViewerViewModel ViewModel {
			get;
		}

		public AlbumViewerViewViewModelPair(string name, UserControl view, IAlbumViewerViewModel viewModel) {
			this.Name = name;
			this.View = view;
			this.ViewModel = viewModel;
		}
	}
}
