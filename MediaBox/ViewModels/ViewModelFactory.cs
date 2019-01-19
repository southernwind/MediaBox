using System;

using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.ViewModels.Tools;

namespace SandBeige.MediaBox.ViewModels {
	internal class ViewModelFactory : FactoryBase<ModelBase, ViewModelBase> {
		public AlbumViewModel Create(AlbumModel album) {
			return this.Create<AlbumModel, AlbumViewModel>(album);
		}

		public MapViewModel Create(MapModel map) {
			return this.Create<MapModel, MapViewModel>(map);
		}

		public MediaFilePropertiesViewModel Create(MediaFileProperties mediaFileProperties) {
			return this.Create<MediaFileProperties, MediaFilePropertiesViewModel>(mediaFileProperties);
		}

		public MediaFileViewModel Create(MediaFileModel mediaFile) {
			return this.Create<MediaFileModel, MediaFileViewModel>(mediaFile);
		}

		public MapPinViewModel Create(MapPin mediaGroup) {
			return this.Create<MapPin, MapPinViewModel>(mediaGroup);
		}

		public AlbumBoxViewModel Create(AlbumBox albumBox) {
			return this.Create<AlbumBox, AlbumBoxViewModel>(albumBox);
		}
		public ExternalToolViewModel Create(ExternalTool model) {
			return this.Create<ExternalTool, ExternalToolViewModel>(model);
		}

		protected override ViewModelBase CreateInstance<TKey, TValue>(TKey key) {
			var instance = Get.Instance<TValue>(key);
			instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
			return instance;
		}
	}
}
