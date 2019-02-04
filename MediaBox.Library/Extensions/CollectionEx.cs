using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class CollectionEx {
		public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items) {
			foreach (var item in items.ToArray()) {
				source.Add(item);
			}
		}

		public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> items) {
			foreach (var item in items.ToArray()) {
				source.Remove(item);
			}
		}
		public static void AddRange<T>(this ICollection<T> source, params T[] items) {
			source.AddRange(items as IEnumerable<T>);
		}

		public static void RemoveRange<T>(this ICollection<T> source, params T[] items) {
			source.RemoveRange(items as IEnumerable<T>);
		}
	}
}