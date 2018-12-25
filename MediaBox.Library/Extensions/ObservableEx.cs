using System;
using System.Reactive.Linq;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class ObservableEx {
		public static IObservable<OldAndNewValue<T>> ToOldAndNewValue<T>(this IObservable<T> p) {
			return p.Zip(p.Skip(1), (x, y) => new OldAndNewValue<T>(x, y));
		}

		public class OldAndNewValue<T> {
			public OldAndNewValue(T oldValue, T newValue) {
				this.OldValue = oldValue;
				this.NewValue = newValue;
			}

			public T OldValue {
				get;
			}

			public T NewValue {
				get;
			}
		}
	}
}
