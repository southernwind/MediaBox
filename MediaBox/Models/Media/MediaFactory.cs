using System;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Media {
	/// <summary>
	/// メディアファクトリー
	/// </summary>
	/// <remarks>
	/// キャッシュ機構を持ったMediaFileModelのファクトリー
	/// ファイルパスをキーとして、同一ファイルを生成したことがあればキャッシュしているものを返す。
	/// GCされていたり、Disposeされていたりすると新しく生成しなおして返す。
	/// </remarks>
	internal class MediaFactory : FactoryBase<string, IMediaFileModel> {
		/// <summary>
		/// <see cref="IMediaFileModel"/>の取得
		/// </summary>
		/// <param name="key">ファイルパス</param>
		/// <returns>生成された<see cref="IMediaFileModel"/></returns>
		public IMediaFileModel Create(string key) {
			var mf = this.Create<string, IMediaFileModel>(key);
			return mf;
		}

		/// <summary>
		/// 新規生成時の関数
		/// </summary>
		/// <typeparam name="TKey">ファイルパスの型(<see cref="string"/>)</typeparam>
		/// <typeparam name="TValue">生成されるインスタンスの型(<see cref="IMediaFileModel"/>)</typeparam>
		/// <param name="key">ファイルパス</param>
		/// <returns>生成された<see cref="IMediaFileModel"/></returns>
		protected override IMediaFileModel CreateInstance<TKey, TValue>(TKey key) {
			// 拡張子で動画か画像かの判定を行う。
			if (key.IsVideoExtension()) {
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