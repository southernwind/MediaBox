using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Map {
	public interface IMapPin : IMediaFileCollection {
		IReactiveProperty<IMediaFileModel> Core {
			get;
		}
		IRectangle CoreRectangle {
			get;
		}
		IReactiveProperty<PinState> PinState {
			get;
		}

		string ToString();
	}
}