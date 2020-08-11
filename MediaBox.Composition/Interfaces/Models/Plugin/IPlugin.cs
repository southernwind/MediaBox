using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Plugin {
	public interface IPlugin {
		string PluginName {
			get;
		}

		PluginType PluginType {
			get;
		}
	}
}
