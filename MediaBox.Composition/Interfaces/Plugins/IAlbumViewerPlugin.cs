using System.Windows.Controls;

using Livet;

namespace SandBeige.MediaBox.Composition.Interfaces.Plugins {
	public interface IAlbumViewerPlugin : IPlugin {
		UserControl CreateViewerControlInstance();

		ViewModel CreateViewModelInstance(IAlbumViewModel album);
	}
}
