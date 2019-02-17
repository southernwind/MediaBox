using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SandBeige.MediaBox.Library.Collection {

	/// <summary>
	/// 双方向同期コレクションインターフェイス
	/// </summary>
	/// <typeparam name="T">要素の型</typeparam>
	public interface ITwoWaySynchronizeCollection<T> : INotifyCollectionChanged, IList<T>, ICollection<T>, ICollection {
		/// <summary>
		/// 追加リクエスト
		/// </summary>
		IObservable<IEnumerable<T>> AddRequestForCollectionChange {
			get;
		}

		/// <summary>
		/// 削除リクエスト
		/// </summary>
		IObservable<IEnumerable<T>> RemoveRequestForCollectionChange {
			get;
		}

		/// <summary>
		/// 追加リクエスト
		/// </summary>
		/// <param name="items">追加するアイテム</param>
		void AddRequest(IEnumerable<T> items);

		/// <summary>
		/// 削除リクエスト
		/// </summary>
		/// <param name="items">削除するアイテム</param>
		void RemoveRequest(IEnumerable<T> items);
	}
}
