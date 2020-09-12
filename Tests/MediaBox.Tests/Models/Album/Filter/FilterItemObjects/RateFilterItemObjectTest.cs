using System;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemObjects {
	internal class RateFilterItemObjectTest : ModelTestClassBase {

		[TestCase(1, SearchTypeComparison.GreaterThan, "評価が1を超える")]
		[TestCase(2, SearchTypeComparison.GreaterThanOrEqual, "評価が2以上")]
		[TestCase(3, SearchTypeComparison.Equal, "評価が3と等しい")]
		[TestCase(4, SearchTypeComparison.LessThanOrEqual, "評価が4以下")]
		[TestCase(5, SearchTypeComparison.LessThan, "評価が5未満")]
		public void プロパティ(int rate, SearchTypeComparison searchType, string displayName) {
			var io = new RateFilterItemObject(rate, searchType);
			io.Rate.Should().Be(rate);
			io.SearchType.Should().Be(searchType);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new RateFilterItemObject();
#pragma warning restore 618
			io2.Rate.Should().Be(0);
			io2.SearchType.Should().Be(SearchTypeComparison.GreaterThan);
			io2.Rate = rate;
			io2.SearchType = searchType;
			io2.Rate.Should().Be(rate);
			io2.SearchType.Should().Be(searchType);
			io2.DisplayName.Should().Be(displayName);
		}

		[TestCase(5, (SearchTypeComparison)100)]
		public void フィルター作成例外(int rate, SearchTypeComparison searchType) {
			Action act = () => {
				_ = new RateFilterItemObject(rate, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
