using System;
using System.Collections.Concurrent;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// サムネイルプール
	/// </summary>
	/// <remarks>
	/// フルサイズのファイルパスをキーにしてサムネイルの管理を行う。
	/// <see cref="ResolveOrRegister(string)"/>で生成したサムネイルはキャッシュされ、以後同じキーで<see cref="ResolveOrRegister(string)"/>された場合
	/// キャッシュされているサムネイルを返却する。
	/// </remarks>
	internal class ThumbnailPool {
		private readonly ConcurrentDictionary<string, (IThumbnail Thumbnail, DateTime LastAccessTime)> _pool;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ThumbnailPool() {
			this._pool = new ConcurrentDictionary<string, (IThumbnail, DateTime)>(6, 100);
		}

		/// <summary>
		/// キーからサムネイルを取得
		/// </summary>
		/// <param name="key">キー</param>
		/// <returns>サムネイル</returns>
		public IThumbnail ResolveOrRegister(string key) {
			var val = this._pool.GetOrAdd(key, k => (Get.Instance<IThumbnail>(), DateTime.Now));
			val.LastAccessTime = DateTime.Now;
			return val.Thumbnail;
		}
	}
}
