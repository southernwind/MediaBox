using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Reactive.Bindings;

namespace SandBeige.MediaBox.Library.Extensions {
	/// <summary>
	/// ObservableCollection拡張クラス
	/// </summary>
	public static class ObservableCollectionEx {
		/// <summary>
		/// 片方向同期
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
		public static IDisposable SynchronizeTo<T>(this ObservableCollection<T> source, IList<T> dest) {
			return source.SynchronizeTo(dest, x => x);
		}

		/// <summary>
		/// 片方向同期
		/// </summary>
		/// <typeparam name="TSource">同期元型</typeparam>
		/// <typeparam name="TDest">同期先型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <param name="selector">変換関数</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
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
