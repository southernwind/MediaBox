using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

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

			collection.Count.Is(4);
			collection.Is(values);

			args.Count.Is(4);
			args.All(x => x.Action == NotifyCollectionChangedAction.Add).IsTrue();
			args.All(x => x.NewItems.Count == 1).IsTrue();
			args.Select(x => x.NewItems[0]).Is(values);

			collection.AddRange(values2);

			collection.Count.Is(8);
			collection.Is(values.Union(values2));
		}
	}
}
