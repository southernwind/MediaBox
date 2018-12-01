using System.Collections.Generic;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class CollectionEx {
		public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items) {
			foreach (var item in items) {
				source.Add(item);
			}
		}
	}
}