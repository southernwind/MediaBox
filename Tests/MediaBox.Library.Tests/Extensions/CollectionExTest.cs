using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

using FluentAssertions;

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
				"D"
			};
			var values2 = new[] {
				"E",
				"F",
				"G",
				"H"
			};
			CollectionEx.AddRange(collection, values);

			collection.Count.Should().Be(4);
			collection.Should().Equal(values);

			args.Count.Should().Be(4);
			args.All(x => x.Action == NotifyCollectionChangedAction.Add).Should().BeTrue();
			args.All(x => x.NewItems.Count == 1).Should().BeTrue();
			// ReSharper disable once CoVariantArrayConversion
			args.Select(x => x.NewItems[0]).Should().Equal(values);

			CollectionEx.AddRange(collection, values2);

			collection.Count.Should().Be(8);
			collection.Should().Equal(values.Union(values2));
		}
	}
}
