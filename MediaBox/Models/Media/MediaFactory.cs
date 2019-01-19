using System;

using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	internal class MediaFactory : FactoryBase<string, MediaFileModel> {
		public MediaFileModel Create(string key) {
			return this.Create<string, MediaFileModel>(key);
		}

		protected override MediaFileModel CreateInstance<TKey, TValue>(TKey key) {
			var instance = Get.Instance<MediaFileModel>(key);
			instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
			return instance;
		}
	}
}