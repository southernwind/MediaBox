using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Plugins;

namespace SandBeige.MediaBox.Plugins.Viewer.TileStyle {
	public class TileStylePlugin : IPlugin {
		public string PluginName {
			get;
		} = "タイルスタイル";

		public PluginType PluginType {
			get;
		} = PluginType.AlbumViewer;
	}
}
