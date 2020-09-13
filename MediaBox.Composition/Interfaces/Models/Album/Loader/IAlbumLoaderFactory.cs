using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader {
	public interface IAlbumLoaderFactory {
		IAlbumLoader Create(IAlbumObject albumObject, IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter);
		IAlbumLoader CreateWithoutSortAndFilter(IAlbumObject albumObject);
	}
}