using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Extensions;

namespace SandBeige.MediaBox.Library.Tests.Extensions {
	[TestFixture]
	internal class CollectionExTest {
		[Test]
		public void AddRange() {
			var collection = new ObservableCollection<string>();
			var args = new List<NotifyCollectionChangedEventArgs>();
			collection.CollectionChanged += (sender, e) => {
				args.Add(e);
			};
			var values = new[]{
				"A",
				"B",
				"C",
				"D",
			};
			var values2 = new[] {
				"E",
				"F",
				"G",
				"H"
			};
			collection.AddRange(values);

			Assert.AreEqual(4, collection.Count);
			CollectionAssert.AreEqual(values, collection);

			Assert.AreEqual(4,args.Count);
			Assert.That(args.All(x => x.Action == NotifyCollectionChangedAction.Add));
			Assert.That(args.All(x => x.NewItems.Count == 1));
			CollectionAssert.AreEqual(values, args.Select(x => x.NewItems[0]));

			collection.AddRange(values2);

			Assert.AreEqual(8, collection.Count);
			CollectionAssert.AreEqual(values.Union(values2), collection);
		}
	}
}
