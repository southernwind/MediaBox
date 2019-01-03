using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SandBeige.MediaBox.Library.Collection {


	public interface ITwoWaySynchronizeCollection<T> : INotifyCollectionChanged, IList<T> {
		IObservable<IEnumerable<T>> AddRequestForCollectionChange {
			get;
		}
		IObservable<IEnumerable<T>> RemoveRequestForCollectionChange {
			get;
		}

		void AddRequest(IEnumerable<T> items);

		void RemoveRequest(IEnumerable<T> items);
	}
}
