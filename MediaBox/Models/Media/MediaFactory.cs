using System;
using System.Collections.Concurrent;

using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	internal class MediaFactory {
		private readonly ConcurrentDictionary<string, MediaFile> _pool;

		public MediaFactory() {
			this._pool = new ConcurrentDictionary<string, MediaFile>(6, 10000);
		}

		public MediaFile Create(string fileName) {
			return this._pool.GetOrAdd(
				fileName,
				key => {
					var instance = Get.Instance<MediaFile>(key);
					instance.OnDisposed.Subscribe(__ => this._pool.TryRemove(instance.FilePath.Value, out _));
					return instance;
				});
		}
	}
}
