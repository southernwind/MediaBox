using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Tool {
	public interface IExternalToolsFactory {
		ReadOnlyReactiveCollection<IExternalTool> Create(string key);
	}
}