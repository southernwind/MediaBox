using System;
using System.Windows.Controls;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IAlbumViewer : IDisposable {
		string Name {
			get;
		}

		UserControl Viewer {
			get;
		}

		IAlbumViewerViewModel ViewModel {
			get;
		}

	}
}