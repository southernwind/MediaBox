using System;

using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	internal class MediaFactory : FactoryBase<string, MediaFile> {
		public MediaFile Create(string key) {
			return this.Create<string, MediaFile>(key);
		}

		protected override MediaFile CreateInstance<TKey, TValue>(TKey key) {
			var instance = Get.Instance<MediaFile>(key);
			instance.OnDisposed.Subscribe(__ => this.Pool.TryRemove(key, out _));
			return instance;
		}
	}
}