
using System;

using Prism.Ioc;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public class AlbumLoaderFactory : IAlbumLoaderFactory {
		private readonly IContainerProvider _containerProvider;
		public AlbumLoaderFactory(IContainerProvider containerProvider) {
			this._containerProvider = containerProvider;
		}

		public IAlbumLoader Create(IAlbumObject albumObject, IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter) {
			IAlbumLoader result = albumObject switch
			{
				FolderAlbumObject => this._containerProvider.Resolve<FolderAlbumLoader>(),
				RegisteredAlbumObject => this._containerProvider.Resolve<RegisteredAlbumLoader>(),
				LookupDatabaseAlbumObject => this._containerProvider.Resolve<LookupDatabaseAlbumLoader>(),
				_ => throw new ArgumentException()
			};
			result.SetAlbumObject(albumObject);
			result.SetFilterAndSort(filterSetter, sortSetter);

			return result;
		}
	}
}