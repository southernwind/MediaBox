using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Viewer {
	public interface IAlbumViewerManager : IModelBase {
		ReactiveCollection<IAlbumViewerViewViewModelPairCreator> AlbumViewerList {
			get;
		}
	}
}