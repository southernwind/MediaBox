using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Plugin {
	public interface IPluginModel : IModelBase {
		IReactiveProperty<bool> IsEnabled {
			get;
		}
		IPlugin PluginInstance {
			get;
		}

		void ToDisable();
		void ToEnable();
	}
}