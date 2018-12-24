using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Library.Tests.Extensions {
	[TestFixture]
	internal class ObservableCollectionExTest {
		[Test]
		public void SynchronizedTo() {
			var collection = new ObservableCollection<int>();
			var target = new List<int>();
			var target2 = new List<int>();
			var disposable = collection.SynchronizeTo(target);
			collection.SynchronizeTo(target2, x => x * 2);

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

			disposable.Dispose();

			collection.Add(3);

			target.Count.Is(0);
			target2.Count.Is(1);
		}


		[Test]
		public void SynchronizedToReadOnly() {
			var sourceCollection = new ObservableCollection<int>();
			var roCollection = new ReadOnlyObservableCollection<int>(sourceCollection);
			var target = new List<int>();
			var target2 = new List<int>();
			var disposable = roCollection.SynchronizeTo(target);
			roCollection.SynchronizeTo(target2, x => x * 2);

			sourceCollection.Add(5);

			target.Count.Is(1);
			target2.Count.Is(1);
			target[0].Is(5);
			target2[0].Is(10);

			sourceCollection.Add(30);

			target.Count.Is(2);
			target2.Count.Is(2);
			target[1].Is(30);
			target2[1].Is(60);

			sourceCollection.Remove(5);

			target.Count.Is(1);
			target2.Count.Is(1);
			target[0].Is(30);
			target2[0].Is(60);

			sourceCollection.Add(8);
			sourceCollection.Add(50);
			sourceCollection.Add(100);

			target.Count.Is(4);
			target2.Count.Is(4);

			sourceCollection.Clear();

			target.Count.Is(0);
			target2.Count.Is(0);

			disposable.Dispose();

			sourceCollection.Add(3);

			target.Count.Is(0);
			target2.Count.Is(1);
		}

		[Test]
		public void DisposeWhenRemove() {
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
			var dwr = collection.DisposeWhenRemove();
			collection.Add(d6);
			collection.Add(d6);
			collection.Remove(d3);
			d3.Disposed.IsTrue();
			d4.Disposed.IsFalse();
			collection.Remove(d4);
			d4.Disposed.IsTrue();
			// 解除
			dwr.Dispose();
			collection.Remove(d5);
			d5.Disposed.IsFalse();
		}

		[Test]
		public void DisposeWhenRemoveReadOnly() {
			var sourceCollection = new ObservableCollection<Disposable>();
			var roCollection = new ReadOnlyObservableCollection<Disposable>(sourceCollection);

			var d1 = new Disposable();
			var d2 = new Disposable();
			var d3 = new Disposable();
			var d4 = new Disposable();
			var d5 = new Disposable();
			var d6 = new Disposable();
			sourceCollection.AddRange(new[] { d1, d2, d3, d4, d5 });
			sourceCollection.Select(x => x.Disposed).Is(Enumerable.Repeat(false, 5));
			sourceCollection.Remove(d2);
			d2.Disposed.IsFalse();
			var dwr = roCollection.DisposeWhenRemove();
			sourceCollection.Add(d6);
			d6.Disposed.IsFalse();
			sourceCollection.Remove(d3);
			d3.Disposed.IsTrue();
			d4.Disposed.IsFalse();
			sourceCollection.Remove(d4);
			d4.Disposed.IsTrue();
			// 解除
			dwr.Dispose();
			sourceCollection.Remove(d5);
			d5.Disposed.IsFalse();
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
