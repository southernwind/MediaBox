using System;

using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels {
	internal class ViewModelFactory : FactoryBase<ModelBase, ViewModelBase> {
		public AlbumViewModel Create(Models.Album.Album album) {
			return this.Create<Models.Album.Album, AlbumViewModel>(album);
		}

		public MapViewModel Create(MapModel map) {
			return this.Create<MapModel, MapViewModel>(map);
		}

		public MediaFilePropertiesViewModel Create(MediaFileProperties mediaFileProperties) {
			return this.Create<MediaFileProperties, MediaFilePropertiesViewModel>(mediaFileProperties);
		}

		public MediaFileViewModel Create(MediaFile mediaFile) {
			return this.Create<MediaFile, MediaFileViewModel>(mediaFile);
		}

		public MediaGroupViewModel Create(MediaGroup mediaGroup) {
			return this.Create<MediaGroup, MediaGroupViewModel>(mediaGroup);
		}

		protected override ViewModelBase CreateInstance<TKey, TValue>(TKey key) {
			var instance = Get.Instance<TValue>(key);
			instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
			return instance;
		}
	}
}
