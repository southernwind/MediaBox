using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box {
	public interface IAlbumBoxSelector {
		IReactiveProperty<IAlbumBox> Shelf {
			get;
		}
	}
}