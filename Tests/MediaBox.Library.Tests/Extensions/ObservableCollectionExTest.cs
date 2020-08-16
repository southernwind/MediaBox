using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

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
				disposable = roCollection.SynchronizeTo<int, ReadOnlyObservableCollection<int>, List<int>>(target);
				roCollection.SynchronizeTo<int, int, ReadOnlyObservableCollection<int>, List<int>>(target2, x => x * 2);
			} else {
				disposable = collection.SynchronizeTo<int, ObservableCollection<int>, List<int>>(target);
				collection.SynchronizeTo<int, int, ObservableCollection<int>, List<int>>(target2, x => x * 2);
			}

			collection.Add(5);

			target.Count.Should().Be(1);
			target2.Count.Should().Be(1);
			target[0].Should().Be(5);
			target2[0].Should().Be(10);

			collection.Add(30);

			target.Count.Should().Be(2);
			target2.Count.Should().Be(2);
			target[1].Should().Be(30);
			target2[1].Should().Be(60);

			collection.Remove(5);

			target.Count.Should().Be(1);
			target2.Count.Should().Be(1);
			target[0].Should().Be(30);
			target2[0].Should().Be(60);

			collection.Add(8);
			collection.Add(50);
			collection.Add(100);

			target.Count.Should().Be(4);
			target2.Count.Should().Be(4);

			collection.Clear();

			target.Count.Should().Be(0);
			target2.Count.Should().Be(0);

			collection.AddRange(new[] { 1, 2, 3, 4, 5 });
			target.Count.Should().Be(5);
			target2.Count.Should().Be(5);
			target.Should().Equal(new[] { 1, 2, 3, 4, 5 });
			target2.Should().Equal(new[] { 2, 4, 6, 8, 10 });

			collection[3] = 18;
			target.Should().Equal(new[] { 1, 2, 3, 18, 5 });
			target2.Should().Equal(new[] { 2, 4, 6, 36, 10 });

			collection.Move(3, 1);
			target.Should().Equal(new[] { 1, 18, 2, 3, 5 });
			target2.Should().Equal(new[] { 2, 36, 4, 6, 10 });

			disposable.Dispose();

			collection.Add(3);

			target.Count.Should().Be(5);
			target2.Count.Should().Be(6);
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
			collection.Select(x => x.Disposed).Should().Equal(Enumerable.Repeat(false, 5));
			collection.Remove(d2);
			d2.Disposed.Should().BeFalse();
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
			d3.Disposed.Should().BeTrue();
			d4.Disposed.Should().BeFalse();
			collection.Remove(d4);
			d4.Disposed.Should().BeTrue();
			// 解除
			disposable.Dispose();
			collection.Remove(d5);
			d5.Disposed.Should().BeFalse();
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
