using System;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IAlbumViewerViewViewModelPair : IDisposable {
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