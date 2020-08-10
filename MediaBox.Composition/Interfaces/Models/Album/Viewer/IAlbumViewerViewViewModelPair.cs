using System.Windows.Controls;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Viewer {
	public interface IAlbumViewerViewViewModelPair : IModelBase {
		string Name {
			get;
		}

		UserControl View {
			get;
		}

		IAlbumViewerViewModel ViewModel {
			get;
		}

	}
}