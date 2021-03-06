using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.Library.Extensions {
	/// <summary>
	/// ObservableCollection拡張クラス
	/// </summary>
	public static class ObservableCollectionEx {
		/// <summary>
		/// 片方向同期
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <typeparam name="TSourceCollection">同期元型</typeparam>
		/// <typeparam name="TDestCollection">同期先型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
		public static IDisposable SynchronizeTo<T, TSourceCollection, TDestCollection>(this TSourceCollection source, TDestCollection dest)
			where TSourceCollection : INotifyCollectionChanged, ICollection<T>, ICollection
			where TDestCollection : IList<T>, ICollection<T>, ICollection {
			return source.SynchronizeTo<T, T, TSourceCollection, TDestCollection>(dest, x => x);
		}

		/// <summary>
		/// 片方向同期
		/// </summary>
		/// <typeparam name="TSource">同期元要素型</typeparam>
		/// <typeparam name="TDest">同期先要素型</typeparam>
		/// <typeparam name="TSourceCollection">同期元型</typeparam>
		/// <typeparam name="TDestCollection">同期先型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <param name="selector">変換関数</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
		public static IDisposable SynchronizeTo<TSource, TDest, TSourceCollection, TDestCollection>(this TSourceCollection source, TDestCollection dest, Func<TSource, TDest> selector)
			where TSourceCollection : INotifyCollectionChanged, ICollection<TSource>, ICollection
			where TDestCollection : IList<TDest>, ICollection<TDest>, ICollection {
			return InnerSynchronizeTo(source, dest, selector);
		}

		/// <summary>
		/// 片方向同期(Private)
		/// </summary>
		/// <typeparam name="TSource">同期元型</typeparam>
		/// <typeparam name="TDest">同期先型</typeparam>
		/// <typeparam name="TSourceCollection">同期元配列型</typeparam>
		/// <typeparam name="TDestCollection">同期先型</typeparam>
		/// <param name="source">同期元</param>
		/// <param name="dest">同期先</param>
		/// <param name="selector">変換関数</param>
		/// <returns><see cref="T:System.IDisposable" />同期終了する場合のDisposeオブジェクト</returns>
		private static IDisposable InnerSynchronizeTo<TSource, TDest, TSourceCollection, TDestCollection>(this TSourceCollection source, TDestCollection dest, Func<TSource, TDest> selector)
			where TSourceCollection : INotifyCollectionChanged, ICollection<TSource>, ICollection
			where TDestCollection : IList<TDest>, ICollection<TDest>, ICollection {
			return source
				.ToCollectionChanged<TSource>()
				.Synchronize()
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
					foreach (var item in x.OldItems.Cast<TSource>()) {
						item?.Dispose();
					}
				});
		}

		public static NotifyCollectionObject<TCollection, T> GetNotifyCollectionObject<TCollection, T>(this TCollection source)
			where TCollection : INotifyCollectionChanged, ICollection<T> {
			List<T> innerList;
			Action<TCollection, NotifyCollectionChangedEventArgs> onCollectionChanged;
			switch (source) {
				case ObservableSynchronizedCollection<T>: {
						innerList = (List<T>)source.GetType().GetField("_list", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(source)!;
						var methodInfo = source.GetType().GetMethod("OnCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance)!;
						onCollectionChanged =
							(Action<TCollection, NotifyCollectionChangedEventArgs>)
							Delegate.CreateDelegate(
								typeof(Action<TCollection, NotifyCollectionChangedEventArgs>),
								methodInfo
							);
						break;
					}
				case ReadOnlyReactiveCollection<T>: {
						// リフレクションキャッシュ生成
						// メディアファイルリストの内部リスト(OC in RORC)
						var oc =
							(ObservableCollection<T>)
							source
								.GetType()
								.GetProperty("Source", BindingFlags.NonPublic | BindingFlags.Instance)!
								.GetValue(source)!;
						// メディアファイルリストの内部リストの更に内部リスト(List in OC)
						innerList =
							(List<T>)
							oc.GetType()
								.GetProperty("Items", BindingFlags.NonPublic | BindingFlags.Instance)!
								.GetValue(oc)!;

						// メディアファイルリストのコレクション変更通知用メソッド
						var methodInfo =
							source
								.GetType()
								.GetMethod("OnCollectionChanged", BindingFlags.NonPublic | BindingFlags.Instance)!;

						onCollectionChanged =
							(Action<TCollection, NotifyCollectionChangedEventArgs>)
							Delegate.CreateDelegate(
								typeof(Action<TCollection, NotifyCollectionChangedEventArgs>),
								methodInfo
							);
					}
					break;
				default:
					throw new ArgumentException("未対応形式");
			}
			return new NotifyCollectionObject<TCollection, T>(innerList, onCollectionChanged);
		}
	}

	public class NotifyCollectionObject<TCollection, T>
		where TCollection : INotifyCollectionChanged, ICollection<T> {
		public IList<T> InnerList {
			get;
		}

		public Action<TCollection, NotifyCollectionChangedEventArgs> OnCollectionChanged {
			get;
		}

		internal NotifyCollectionObject(IList<T> innerList, Action<TCollection, NotifyCollectionChangedEventArgs> onCollectionChanged) {
			this.InnerList = innerList;
			this.OnCollectionChanged = onCollectionChanged;
		}
	}
}
