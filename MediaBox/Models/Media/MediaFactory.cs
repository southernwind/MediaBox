using System;
using System.IO;

using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	internal class MediaFactory : FactoryBase<string, MediaFileModel> {
		public MediaFileModel Create(string key, ThumbnailLocation location = ThumbnailLocation.None) {
			var mf = this.Create<string, MediaFileModel>(key);
			mf.ThumbnailLocation |= location;
			return mf;
		}

		protected override MediaFileModel CreateInstance<TKey, TValue>(TKey key) {
			if (Path.GetExtension(key).ToLower() == ".mov") {
				var instance = Get.Instance<VideoFileModel>(key);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			} else {
				var instance = Get.Instance<ImageFileModel>(key);
				instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
				return instance;
			}
		}
	}
}