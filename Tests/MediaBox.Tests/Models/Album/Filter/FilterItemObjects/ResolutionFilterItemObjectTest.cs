using System;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemObjects {
	internal class ResolutionFilterItemObjectTest : ModelTestClassBase {
		[TestCase(10, 10, SearchTypeComparison.GreaterThan, "解像度が10x10を超える")]
		[TestCase(20, 10, SearchTypeComparison.GreaterThanOrEqual, "解像度が20x10以上")]
		[TestCase(10, 30, SearchTypeComparison.Equal, "解像度が10x30と等しい")]
		[TestCase(40, 10, SearchTypeComparison.LessThanOrEqual, "解像度が40x10以下")]
		[TestCase(10, 50, SearchTypeComparison.LessThan, "解像度が10x50未満")]
		public void プロパティ面積パターン(double width, double height, SearchTypeComparison searchType, string displayName) {
			var io = new ResolutionFilterItemObject(new ComparableSize(width, height), searchType);
			io.Width.Should().BeNull();
			io.Height.Should().BeNull();
			io.Resolution.Should().Be(new ComparableSize(width, height));
			io.SearchType.Should().Be(searchType);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new ResolutionFilterItemObject();
#pragma warning restore 618
			io2.Width.Should().BeNull();
			io2.Height.Should().BeNull();
			io2.Resolution.Should().BeNull();
			io2.SearchType.Should().Be(SearchTypeComparison.GreaterThan);
			io2.Resolution = new ComparableSize(width, height);
			io2.SearchType = searchType;
			io2.Resolution.Should().Be(new ComparableSize(width, height));
			io2.SearchType.Should().Be(searchType);
			io2.DisplayName.Should().Be(displayName);
		}

		[TestCase(10, null, SearchTypeComparison.GreaterThan, "幅が10を超える")]
		[TestCase(20, null, SearchTypeComparison.GreaterThanOrEqual, "幅が20以上")]
		[TestCase(30, null, SearchTypeComparison.Equal, "幅が30と等しい")]
		[TestCase(40, null, SearchTypeComparison.LessThanOrEqual, "幅が40以下")]
		[TestCase(50, null, SearchTypeComparison.LessThan, "幅が50未満")]
		[TestCase(null, 10, SearchTypeComparison.GreaterThan, "高さが10を超える")]
		[TestCase(null, 20, SearchTypeComparison.GreaterThanOrEqual, "高さが20以上")]
		[TestCase(null, 30, SearchTypeComparison.Equal, "高さが30と等しい")]
		[TestCase(null, 40, SearchTypeComparison.LessThanOrEqual, "高さが40以下")]
		[TestCase(null, 50, SearchTypeComparison.LessThan, "高さが50未満")]
		public void プロパティ幅高さパターン(int? width, int? height, SearchTypeComparison searchType, string displayName) {
			var io = new ResolutionFilterItemObject(width, height, searchType);
			io.Width.Should().Be(width);
			io.Height.Should().Be(height);
			io.Resolution.Should().BeNull();
			io.SearchType.Should().Be(searchType);
			io.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var io2 = new ResolutionFilterItemObject();
#pragma warning restore 618
			io2.Width.Should().BeNull();
			io2.Height.Should().BeNull();
			io2.Resolution.Should().BeNull();
			Assert.Throws<InvalidOperationException>(() => {
				_ = io2.DisplayName;
			});
			io2.SearchType.Should().Be(SearchTypeComparison.GreaterThan);
			io2.Width = width;
			io2.Height = height;
			io2.SearchType = searchType;
			io2.Width.Should().Be(width);
			io2.Height.Should().Be(height);
			io2.SearchType.Should().Be(searchType);
			io2.DisplayName.Should().Be(displayName);
		}

		[TestCase(10, 10, (SearchTypeInclude)100)]
		public void フィルター作成例外面積パターン(double width, double height, SearchTypeComparison searchType) {
			Assert.Throws<ArgumentException>(() => {
				_ = new ResolutionFilterItemObject(new ComparableSize(width, height), searchType);
			});
		}

		[TestCase(null, null, SearchTypeComparison.GreaterThan)]
		[TestCase(null, null, SearchTypeComparison.GreaterThanOrEqual)]
		[TestCase(null, null, SearchTypeComparison.Equal)]
		[TestCase(null, null, SearchTypeComparison.LessThanOrEqual)]
		[TestCase(null, null, SearchTypeComparison.LessThan)]
		[TestCase(10, 10, SearchTypeComparison.GreaterThan)]
		[TestCase(10, 10, SearchTypeComparison.GreaterThanOrEqual)]
		[TestCase(10, 10, SearchTypeComparison.Equal)]
		[TestCase(10, 10, SearchTypeComparison.LessThanOrEqual)]
		[TestCase(10, 10, SearchTypeComparison.LessThan)]
		[TestCase(10, null, (SearchTypeComparison)100)]
		public void フィルター作成例外幅高さパターン(int? width, int? height, SearchTypeComparison searchType) {
			Action act = () => {
				_ = new ResolutionFilterItemObject(width, height, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
