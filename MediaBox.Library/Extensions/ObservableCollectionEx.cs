using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Library.Collection;

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
		/// <typeparam name="T">型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
		public static IDisposable SynchronizeTo<T>(this ReadOnlyObservableCollection<T> source, IList<T> dest) {
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
			return InnerSynchronizeTo(source, dest, selector);
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
		public static IDisposable SynchronizeTo<TSource, TDest>(this ReadOnlyObservableCollection<TSource> source, IList<TDest> dest, Func<TSource, TDest> selector) {
			return InnerSynchronizeTo(source, dest, selector);
		}

		/// <summary>
		/// 片方向同期(Private)
		/// </summary>
		/// <typeparam name="TSource">同期元型</typeparam>
		/// <typeparam name="TDest">同期先型</typeparam>
		/// <typeparam name="TSourceCollection">同期元配列型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <param name="selector">変換関数</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
		private static IDisposable InnerSynchronizeTo<TSource, TDest, TSourceCollection>(this TSourceCollection source, IList<TDest> dest, Func<TSource, TDest> selector)
			where TSourceCollection : INotifyCollectionChanged, ICollection<TSource> {
			return source
				.ToCollectionChanged<TSource>()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							dest.Insert(x.Index, selector(x.Value));
							break;
						case NotifyCollectionChangedAction.Remove:
							dest.RemoveAt(x.Index);
							break;
						case NotifyCollectionChangedAction.Reset:
							dest.Clear();
							dest.AddRange(source.Select(selector));
							break;
						case NotifyCollectionChangedAction.Replace:
							dest[x.Index] = selector(x.Value);
							break;
						case NotifyCollectionChangedAction.Move:
							var old = dest[x.OldIndex];
							dest.Remove(old);
							dest.Insert(x.Index, old);
							break;
					}
				});
		}

		/// <summary>
		/// 双方向同期
		/// </summary>
		/// <typeparam name="TSource">同期元型</typeparam>
		/// <typeparam name="TDest">同期先型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <param name="selector">同期元→同期先変換関数</param>
		/// <param name="selector2">同期先→同期元変換関数</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
		public static IDisposable TwoWaySynchronizeTo<TSource, TDest>(this ITwoWaySynchronizeCollection<TSource> source, ITwoWaySynchronizeCollection<TDest> dest, Func<TSource, TDest> selector, Func<TDest, TSource> selector2) {
			var disposable = new CompositeDisposable();

			dest.AddRequestForCollectionChange.Subscribe(x => {
				source.AddRange(x.Select(selector2));
			}).AddTo(disposable);

			dest.RemoveRequestForCollectionChange.Subscribe(x => {
				foreach (var item in x.Select(selector2)) {
					source.Remove(item);
				}
			}).AddTo(disposable);

			source.InnerSynchronizeTo(dest, selector).AddTo(disposable);

			return disposable;
		}

		/// <summary>
		/// 配列から消えたとき、Disposeする
		/// </summary>
		/// <typeparam name="TSource">配列要素型</typeparam>
		/// <param name="source">対象配列</param>
		/// <returns><see cref="T:System.IDisposable" />Disposeを中止する場合のDisposeオブジェクト</returns>
		public static IDisposable DisposeWhenRemove<TSource>(this ReadOnlyObservableCollection<TSource> source)
			where TSource : IDisposable {
			return InnerDisposeWhenRemove<TSource, ReadOnlyObservableCollection<TSource>>(source);
		}

		/// <summary>
		/// 配列から消えたとき、Disposeする
		/// </summary>
		/// <typeparam name="TSource">配列要素型</typeparam>
		/// <param name="source">対象配列</param>
		/// <returns><see cref="T:System.IDisposable" />Disposeを中止する場合のDisposeオブジェクト</returns>
		public static IDisposable DisposeWhenRemove<TSource>(this ObservableCollection<TSource> source)
			where TSource : IDisposable {
			return InnerDisposeWhenRemove<TSource, ObservableCollection<TSource>>(source);
		}

		/// <summary>
		/// 配列から消えたとき、Disposeする(Private)
		/// </summary>
		/// <typeparam name="TSource">配列要素型</typeparam>
		/// <typeparam name="TCollection">配列型</typeparam>
		/// <param name="source">対象配列</param>
		/// <returns><see cref="T:System.IDisposable" />Disposeを中止する場合のDisposeオブジェクト</returns>
		private static IDisposable InnerDisposeWhenRemove<TSource, TCollection>(this TCollection source)
			where TSource : IDisposable
			where TCollection : INotifyCollectionChanged, ICollection<TSource> {
			return source
				.CollectionChangedAsObservable()
				.Subscribe(x => {
					if (x.OldItems == null) {
						return;
					}
					foreach (var item in x.OldItems.OfType<TSource>()) {
						item.Dispose();
					}
				});
		}
	}
}
