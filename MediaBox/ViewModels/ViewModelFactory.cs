using System;
using System.Collections.Concurrent;

using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels {
	internal class ViewModelFactory {
		private readonly ConcurrentDictionary<ModelBase, ViewModelBase> _pool;

		public ViewModelFactory() {
			this._pool = new ConcurrentDictionary<ModelBase, ViewModelBase>(6, 10000);
		}

		public AlbumViewModel Create(Models.Album.Album album) {
			return this.Create<AlbumViewModel, Models.Album.Album>(album);
		}

		public MapViewModel Create(MapModel map) {
			return this.Create<MapViewModel, MapModel>(map);
		}

		public MediaFilePropertiesViewModel Create(MediaFileProperties mediaFileProperties) {
			return this.Create<MediaFilePropertiesViewModel, MediaFileProperties>(mediaFileProperties);
		}

		public MediaFileViewModel Create(MediaFile mediaFile) {
			return this.Create<MediaFileViewModel, MediaFile>(mediaFile);
		}

		public MediaGroupViewModel Create(MediaGroup mediaGroup) {
			return this.Create<MediaGroupViewModel, MediaGroup>(mediaGroup);
		}

		public TVM Create<TVM, TM>(TM model)
			where TVM : ViewModelBase
			where TM : ModelBase {
			return (TVM)this._pool.GetOrAdd(
				model,
				key => {
					var instance = Get.Instance<TVM>(model);
					instance.OnDisposed.Subscribe(__ => this._pool.TryRemove(key, out _));
					return instance;
				});
		}
	}
}
