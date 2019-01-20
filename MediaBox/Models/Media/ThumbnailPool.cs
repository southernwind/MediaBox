﻿using System;
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
		/// キーからサムネイルを取得、できない場合はキーをフルサイズファイルパスとしてサムネイルインスタンスを生成する
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="location">サムネイル作成場所</param>
		/// <returns>サムネイル</returns>
		public Thumbnail ResolveOrRegisterByFullSizeFilePath(string key, ThumbnailLocation location) {
			var thumbnail = this.ResolveOrRegister(
				key,
				k => {
					var thumb = Get.Instance<Thumbnail>();
					thumb.FullSizeFilePath = k;
					thumb.CreateThumbnail(location);
					return thumb;
				});
			if (!thumbnail.Location.HasFlag(location)) {
				thumbnail.CreateThumbnail(location);
			}
			return thumbnail;
		}

		/// <summary>
		/// キーからサムネイルを取得、できない場合は引数のサムネイルファイル名を使ってサムネイルインスタンスを生成する
		/// </summary>
		/// <param name="key">キー</param>
		/// <param name="thumbnailFileName">サムネイルファイル名</param>
		/// <returns>サムネイル</returns>

		public Thumbnail ResolveOrRegisterByThumbnailFileName(string key, string thumbnailFileName) {
			return this.ResolveOrRegister(
				key,
				k => {
					var thumb = Get.Instance<Thumbnail>();
					thumb.FileName = thumbnailFileName;
					return thumb;
				});
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
		private Thumbnail ResolveOrRegister(string key, Func<string, Thumbnail> valueFactory) {
			var thumbnail = this.Resolve(key);
			if (thumbnail != null) {
				return thumbnail;
			}

			thumbnail = valueFactory(key);
			this.Register(key, thumbnail);
			return thumbnail;
		}
	}
}
