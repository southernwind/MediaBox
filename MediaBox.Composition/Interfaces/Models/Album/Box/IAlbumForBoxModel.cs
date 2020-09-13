
using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box {
	public interface IAlbumForBoxModel : IModelBase {
		public IReactiveProperty<string> Title {
			get;
		}

		public IReactiveProperty<int> Count {
			get;
		}

		public IReactiveProperty<int?> AlbumBoxId {
			get;
		}

		public IAlbumObject AlbumObject {
			get;
		}
	}
}
