using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Library.Extensions {
	/// <summary>
	/// <see cref="ConcurrentQueue{T}"/>の拡張メソッドクラス
	/// </summary>
	public static class ConcurrentQueueEx {
		/// <summary>
		/// 複数追加
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">追加先コレクション</param>
		/// <param name="items">追加するコレクション</param>
		public static void EnqueueRange<T>(this ConcurrentQueue<T> queue, IEnumerable<T> items) {
			foreach (var item in items.ToArray()) {
				queue.Enqueue(item);
			}
		}
	}
}
