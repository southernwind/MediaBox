using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Plugins;

namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle {
	public class TileStylePlugin : IAlbumViewerPlugin {
		public string PluginName {
			get;
		} = "サムネイル3枚";

		public PluginType PluginType {
			get;
		} = PluginType.AlbumViewer;

		public UserControl CreateViewerControlInstance() {
			return new Viewer();
		}

		public IAlbumViewerViewModel CreateViewModelInstance(IAlbumViewModel album) {
			return new ViewerViewModel(album);
		}
	}
}
