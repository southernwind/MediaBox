using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class ConcurrentQueueEx {
		public static void EnqueueRange<T>(this ConcurrentQueue<T> queue, IEnumerable<T> items) {
			foreach (var item in items.ToArray()) {
				queue.Enqueue(item);
			}
		}
	}
}
