using System.Windows.Controls;

namespace SandBeige.MediaBox.Composition.Interfaces.Plugins {
	public interface IAlbumViewerPlugin : IPlugin {
		UserControl CreateViewerControlInstance();
	}
}
