using System;
using System.Windows.Controls;

using Livet;

namespace SandBeige.MediaBox.Composition.Interfaces {
	public interface IAlbumViewer : IDisposable {
		string Name {
			get;
		}

		UserControl Viewer {
			get;
		}

		ViewModel ViewModel {
			get;
		}

	}
}