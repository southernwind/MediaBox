using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box {
	public interface IAlbumBoxSelector : IModelBase {
		IReactiveProperty<IAlbumBox> Shelf {
			get;
		}
	}
}