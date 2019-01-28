using System;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class ObjectEx {
		public static T2 Lock<T, T2>(this T obj, Func<T, T2> func) {
			lock (obj) {
				return func(obj);
			}
		}

		public static void Lock<T>(this T obj, Action<T> func) {
			lock (obj) {
				func(obj);
			}
		}
	}
}
