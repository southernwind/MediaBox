using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイルプール
	/// </summary>
	class ThumbnailPool {
		private readonly ConcurrentDictionary<string, (byte[] Binary, DateTime LastAccessTime)> _pool;

		public ThumbnailPool() {
			this._pool = new ConcurrentDictionary<string, (byte[], DateTime)>(6, 100);
		}

		/// <summary>
		/// サムネイル登録
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="binary">サムネイル</param>
		public void Register(string key,byte[] binary) {
			this._pool.TryAdd(key, (binary, DateTime.Now));
		}

		/// <summary>
		/// キーからサムネイルを取得する
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>サムネイル、登録されていなければnull</returns>
		public byte[] Resolve(string key) {
			if (this._pool.TryGetValue(key,out var item)) {
				item.LastAccessTime = DateTime.Now;
			}
			return item.Binary;
		}

		/// <summary>
		/// キーからサムネイルを取得、生成関数で生成して登録
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="func">生成関数</param>
		/// <returns>サムネイル</returns>
		public byte[] ResolveOrRegister(string key, Func<byte[]> valueFactory) {
			var thumbnail = this.Resolve(key);
			if (thumbnail == null) {
				thumbnail = valueFactory();
				this.Register(key, thumbnail);
			}
			return thumbnail;
		}
	}
}
