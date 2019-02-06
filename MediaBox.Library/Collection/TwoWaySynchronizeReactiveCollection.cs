using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Library.Collection {
	/// <summary>
	/// 双方向同期可能なReactiveCollection
	/// </summary>
	/// <typeparam name="T">型</typeparam>
	public class TwoWaySynchronizeReactiveCollection<T> : ReactiveCollection<T>, ITwoWaySynchronizeCollection<T> {
		/// <summary>
		/// 追加リクエスト用Subject
		/// </summary>
		private readonly Subject<IEnumerable<T>> _addRequestForCollectionChange = new Subject<IEnumerable<T>>();

		/// <summary>
		/// 削除リクエスト用Subject
		/// </summary>
		private readonly Subject<IEnumerable<T>> _removeRequestForCollectionChange = new Subject<IEnumerable<T>>();

		/// <summary>
		/// 追加リクエスト
		/// </summary>
		public IObservable<IEnumerable<T>> AddRequestForCollectionChange {
			get {
				return this._addRequestForCollectionChange.AsObservable();
			}
		}

		/// <summary>
		/// 削除リクエスト
		/// </summary>
		public IObservable<IEnumerable<T>> RemoveRequestForCollectionChange {
			get {
				return this._removeRequestForCollectionChange.AsObservable();
			}
		}

		/// <summary>
		/// 追加リクエスト
		/// </summary>
		/// <param name="items"></param>
		public void AddRequest(IEnumerable<T> items) {
			this._addRequestForCollectionChange.OnNext(items);
		}

		/// <summary>
		/// 削除リクエスト
		/// </summary>
		/// <param name="items"></param>
		public void RemoveRequest(IEnumerable<T> items) {
			this._removeRequestForCollectionChange.OnNext(items);
		}
	}
}
