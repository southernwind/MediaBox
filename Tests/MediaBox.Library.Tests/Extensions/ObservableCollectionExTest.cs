using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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
	}
}
