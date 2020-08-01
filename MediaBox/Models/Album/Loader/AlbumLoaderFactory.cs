
using System;

using Prism.Ioc;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.Loader {
	public class AlbumLoaderFactory {
		private readonly IContainerProvider _containerProvider;
		public AlbumLoaderFactory(IContainerProvider containerProvider) {
			this._containerProvider = containerProvider;
		}

		public AlbumLoader Create(IAlbumObject albumObject) {
			switch (albumObject) {
				case FolderAlbumObject fao:
					var fal = this._containerProvider.Resolve<FolderAlbumLoader>();
					fal.SetAlbumObject(fao);
					return fal;
				case RegisteredAlbumObject rao:
					var ral = this._containerProvider.Resolve<RegisteredAlbumLoader>();
					ral.SetAlbumObject(rao);
					return ral;
				case LookupDatabaseAlbumObject ldao:
					var ldal = this._containerProvider.Resolve<LookupDatabaseAlbumLoader>();
					ldal.SetAlbumObject(ldao);
					return ldal;
				default:
					throw new ArgumentException();
			}
		}
	}
}
