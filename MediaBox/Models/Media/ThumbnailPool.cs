using System;
using System.Collections.Concurrent;

using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイルプール
	/// </summary>
	internal class ThumbnailPool {
		private readonly ConcurrentDictionary<string, (Thumbnail Thumbnail, DateTime LastAccessTime)> _pool;

		public ThumbnailPool() {
			this._pool = new ConcurrentDictionary<string, (Thumbnail, DateTime)>(6, 100);
		}

		/// <summary>
		/// サムネイル登録
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="binary">サムネイル</param>
		private void Register(string key, Thumbnail binary) {
			this._pool.TryAdd(key, (binary, DateTime.Now));
		}

		/// <summary>
		/// キーからサムネイルを取得する
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>サムネイル、登録されていなければnull</returns>
		private Thumbnail Resolve(string key) {
			if (this._pool.TryGetValue(key, out var item)) {
				item.LastAccessTime = DateTime.Now;
			}
			return item.Thumbnail;
		}

		/// <summary>
		/// キーからサムネイルを取得、生成関数で生成して登録
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="valueFactory">生成関数</param>
		/// <returns>サムネイル</returns>
		public Thumbnail ResolveOrRegister(string key) {
			var thumbnail = this.Resolve(key);
			if (thumbnail != null) {
				return thumbnail;
			}

			thumbnail = Get.Instance<Thumbnail>();
			this.Register(key, thumbnail);
			return thumbnail;
		}
	}
}
