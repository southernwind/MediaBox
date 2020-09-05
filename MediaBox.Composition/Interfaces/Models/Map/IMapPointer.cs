using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Map {
	public interface IMapPointer : IMediaFileCollection {
		IReactiveProperty<IMediaFileModel> Core {
			get;
		}

		string ToString();
	}
}