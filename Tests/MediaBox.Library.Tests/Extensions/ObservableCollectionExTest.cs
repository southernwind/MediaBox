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

			Assert.AreEqual(1, target.Count);
			Assert.AreEqual(1, target2.Count);
			Assert.AreEqual(5, target[0]);
			Assert.AreEqual(10, target2[0]);

			collection.Add(30);

			Assert.AreEqual(2, target.Count);
			Assert.AreEqual(2, target2.Count);
			Assert.AreEqual(30, target[1]);
			Assert.AreEqual(60, target2[1]);

			collection.Remove(5);

			Assert.AreEqual(1, target.Count);
			Assert.AreEqual(1, target2.Count);
			Assert.AreEqual(30, target[0]);
			Assert.AreEqual(60, target2[0]);

			collection.Add(8);
			collection.Add(50);
			collection.Add(100);

			Assert.AreEqual(4, target.Count);
			Assert.AreEqual(4, target2.Count);

			collection.Clear();

			Assert.AreEqual(0, target.Count);
			Assert.AreEqual(0, target2.Count);

			disposable.Dispose();

			collection.Add(3);

			Assert.AreEqual(0, target.Count);
			Assert.AreEqual(1, target2.Count);
		}
	}
}
