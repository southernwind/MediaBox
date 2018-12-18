using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Reactive.Bindings;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class ObservableCollectionEx {
		public static IDisposable SynchronizeTo<T>(this ObservableCollection<T> source, IList<T> dest) {
			return source.SynchronizeTo(dest, x => x);
		}

		public static IDisposable SynchronizeTo<TSource, TDest>(this ObservableCollection<TSource> source, IList<TDest> dest, Func<TSource, TDest> selector) {
			return source
				.ToCollectionChanged()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							dest.Add(selector(x.Value));
							break;
						case NotifyCollectionChangedAction.Remove:
							dest.Remove(selector(x.Value));
							break;
						case NotifyCollectionChangedAction.Reset:
							dest.Clear();
							dest.AddRange(source.Select(selector));
							break;
					}
				});
		}
	}
}
