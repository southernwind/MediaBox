using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Plugin {
	public interface IPluginManager {
		ReadOnlyReactiveCollection<IPluginModel> PluginList {
			get;
		}
	}
}