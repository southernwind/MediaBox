using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Interfaces.Plugins {
	public interface IPlugin {
		string PluginName {
			get;
		}

		PluginType PluginType {
			get;
		}
	}
}
