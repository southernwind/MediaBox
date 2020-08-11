using System.Windows.Controls;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Plugin {
	public interface IAlbumViewerPlugin : IPlugin {
		UserControl CreateViewerControlInstance();

		IAlbumViewerViewModel CreateViewModelInstance(IAlbumViewModel album);
	}
}
