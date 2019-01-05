using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Library.Collection;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Library.Tests.Extensions {
	[TestFixture]
	internal class ObservableCollectionExTest {
		[TestCase(true)]
		[TestCase(false)]
		public void SynchronizedTo(bool readOnly) {
			var collection = new ObservableCollection<int>();
			var target = new List<int>();
			var target2 = new List<int>();
			IDisposable disposable;
			if (readOnly) {
				var roCollection = new ReadOnlyObservableCollection<int>(collection);
				disposable = roCollection.SynchronizeTo(target);
				roCollection.SynchronizeTo(target2, x => x * 2);
			} else {
				disposable = collection.SynchronizeTo(target);
				collection.SynchronizeTo(target2, x => x * 2);
			}

			collection.Add(5);

			target.Count.Is(1);
			target2.Count.Is(1);
			target[0].Is(5);
			target2[0].Is(10);

			collection.Add(30);

			target.Count.Is(2);
			target2.Count.Is(2);
			target[1].Is(30);
			target2[1].Is(60);

			collection.Remove(5);

			target.Count.Is(1);
			target2.Count.Is(1);
			target[0].Is(30);
			target2[0].Is(60);

			collection.Add(8);
			collection.Add(50);
			collection.Add(100);

			target.Count.Is(4);
			target2.Count.Is(4);

			collection.Clear();

			target.Count.Is(0);
			target2.Count.Is(0);

			collection.AddRange(new[] { 1, 2, 3, 4, 5 });
			target.Count.Is(5);
			target2.Count.Is(5);
			target.Is(1, 2, 3, 4, 5);
			target2.Is(2, 4, 6, 8, 10);

			collection[3] = 18;
			target.Is(1, 2, 3, 18, 5);
			target2.Is(2, 4, 6, 36, 10);

			collection.Move(3, 1);
			target.Is(1, 18, 2, 3, 5);
			target2.Is(2, 36, 4, 6, 10);

			disposable.Dispose();

			collection.Add(3);

			target.Count.Is(5);
			target2.Count.Is(6);
		}

		[TestCase(true)]
		[TestCase(false)]
		public void DisposeWhenRemove(bool readOnly) {
			var collection = new ObservableCollection<Disposable>();

			var d1 = new Disposable();
			var d2 = new Disposable();
			var d3 = new Disposable();
			var d4 = new Disposable();
			var d5 = new Disposable();
			var d6 = new Disposable();
			collection.AddRange(new[] { d1, d2, d3, d4, d5 });
			collection.Select(x => x.Disposed).Is(Enumerable.Repeat(false, 5));
			collection.Remove(d2);
			d2.Disposed.IsFalse();
			IDisposable disposable;
			if (readOnly) {
				var roCollection = new ReadOnlyObservableCollection<Disposable>(collection);
				disposable = roCollection.DisposeWhenRemove();
			} else {
				disposable = collection.DisposeWhenRemove();
			}
			collection.Add(d6);
			collection.Add(d6);
			collection.Remove(d3);
			d3.Disposed.IsTrue();
			d4.Disposed.IsFalse();
			collection.Remove(d4);
			d4.Disposed.IsTrue();
			// 解除
			disposable.Dispose();
			collection.Remove(d5);
			d5.Disposed.IsFalse();
		}

		[Test]
		public void TwoWaySynchronizeCollection() {
			var collection1 = new TwoWaySynchronizeReactiveCollection<int>();
			var collection2 = new TwoWaySynchronizeReactiveCollection<string>();
			var disposable = collection1.TwoWaySynchronizeTo(collection2, x => x.ToString(), int.Parse);

			collection1.Add(1);
			collection1.Add(2);

			collection2.Is("1", "2");

			collection1.Remove(1);

			collection2.Is("2");

			collection2.AddRequest(new[] { "3", "4" });

			collection1.Is(2, 3, 4);
			collection2.Is("2", "3", "4");

			collection2.RemoveRequest(new[] { "2", "4" });

			collection1.Is(3);
			collection2.Is("3");

			disposable.Dispose();

			collection1.Add(4);
			collection1.Is(3, 4);
			collection2.Is("3");

			collection2.AddRequest(new[] { "5" });

			collection1.Is(3, 4);
			collection2.Is("3");

			collection2.RemoveRequest(new[] { "3" });
			collection1.Is(3, 4);
			collection2.Is("3");

			collection1.Remove(3);
			collection1.Is(4);
			collection2.Is("3");
		}
	}

	public class Disposable : IDisposable {
		public bool Disposed {
			get;
			private set;
		}

		public void Dispose() {
			this.Disposed = true;
		}
	}
}
