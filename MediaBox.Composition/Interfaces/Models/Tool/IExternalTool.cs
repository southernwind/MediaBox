using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Tool {
	public interface IExternalTool : IModelBase {
		IReadOnlyReactiveProperty<string> Arguments {
			get;
			set;
		}
		IReadOnlyReactiveProperty<string> Command {
			get;
			set;
		}
		IReadOnlyReactiveProperty<string> DisplayName {
			get;
			set;
		}
		ReadOnlyReactiveCollection<string> TargetExtensions {
			get;
			set;
		}

		void Start(string filename);
		string ToString();
	}
}