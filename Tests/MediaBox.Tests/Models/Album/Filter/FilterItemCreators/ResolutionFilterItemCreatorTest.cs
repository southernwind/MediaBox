using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class ResolutionFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				new MediaFile { FilePath = this.TestFiles.Image1Jpg.FilePath, MediaFileId = 1, Width = 10, Height = 10 },
				new MediaFile { FilePath = this.TestFiles.Image2Jpg.FilePath, MediaFileId = 2, Width = 11, Height = 9 },
				new MediaFile { FilePath = this.TestFiles.Image3Jpg.FilePath, MediaFileId = 3, Width = 9, Height = 11 },
				new MediaFile { FilePath = this.TestFiles.Image4Png.FilePath, MediaFileId = 4, Width = 100, Height = 1 },
				new MediaFile { FilePath = this.TestFiles.NoExifJpg.FilePath, MediaFileId = 5, Width = 1, Height = 100 },
				new MediaFile { FilePath = this.TestFiles.Video1Mov.FilePath, MediaFileId = 6, Width = 500, Height = 3000 }
			};
			this.CreateModels();
		}

		[TestCase(10, 10, SearchTypeComparison.GreaterThan, "解像度が10x10を超える")]
		[TestCase(20, 10, SearchTypeComparison.GreaterThanOrEqual, "解像度が20x10以上")]
		[TestCase(10, 30, SearchTypeComparison.Equal, "解像度が10x30と等しい")]
		[TestCase(40, 10, SearchTypeComparison.LessThanOrEqual, "解像度が40x10以下")]
		[TestCase(10, 50, SearchTypeComparison.LessThan, "解像度が10x50未満")]
		public void プロパティ面積パターン(double width, double height, SearchTypeComparison searchType, string displayName) {
			var ic = new ResolutionFilterItemCreator(new ComparableSize(width, height), searchType);
			ic.Width.Should().BeNull();
			ic.Height.Should().BeNull();
			ic.Resolution.Should().Be(new ComparableSize(width, height));
			ic.SearchType.Should().Be(searchType);
			ic.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var ic2 = new ResolutionFilterItemCreator();
#pragma warning restore 618
			ic2.Width.Should().BeNull();
			ic2.Height.Should().BeNull();
			ic2.Resolution.Should().BeNull();
			ic2.SearchType.Should().Be(SearchTypeComparison.GreaterThan);
			ic2.Resolution = new ComparableSize(width, height);
			ic2.SearchType = searchType;
			ic2.Resolution.Should().Be(new ComparableSize(width, height));
			ic2.SearchType.Should().Be(searchType);
			ic2.DisplayName.Should().Be(displayName);
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
			var ic = new ResolutionFilterItemCreator(width, height, searchType);
			ic.Width.Should().Be(width);
			ic.Height.Should().Be(height);
			ic.Resolution.Should().BeNull();
			ic.SearchType.Should().Be(searchType);
			ic.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var ic2 = new ResolutionFilterItemCreator();
#pragma warning restore 618
			ic2.Width.Should().BeNull();
			ic2.Height.Should().BeNull();
			ic2.Resolution.Should().BeNull();
			Assert.Throws<InvalidOperationException>(() => {
				_ = ic2.DisplayName;
			});
			ic2.SearchType.Should().Be(SearchTypeComparison.GreaterThan);
			ic2.Width = width;
			ic2.Height = height;
			ic2.SearchType = searchType;
			ic2.Width.Should().Be(width);
			ic2.Height.Should().Be(height);
			ic2.SearchType.Should().Be(searchType);
			ic2.DisplayName.Should().Be(displayName);
		}

		[TestCase(10, 10, SearchTypeComparison.GreaterThan, 6)]
		[TestCase(10, 10, SearchTypeComparison.GreaterThanOrEqual, 1, 4, 5, 6)]
		[TestCase(10, 10, SearchTypeComparison.Equal, 1, 4, 5)]
		[TestCase(10, 10, SearchTypeComparison.LessThanOrEqual, 1, 2, 3, 4, 5)]
		[TestCase(10, 10, SearchTypeComparison.LessThan, 2, 3)]
		[TestCase(9, 11, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 3, 4, 5, 6)]
		[TestCase(11, 9, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 3, 4, 5, 6)]
		[TestCase(1, 100, SearchTypeComparison.GreaterThanOrEqual, 1, 4, 5, 6)]
		[TestCase(1, 101, SearchTypeComparison.GreaterThanOrEqual, 6)]
		[TestCase(1, 1500000, SearchTypeComparison.GreaterThanOrEqual, 6)]
		[TestCase(1, 1500001, SearchTypeComparison.GreaterThanOrEqual)]
		public void フィルタリング面積パターン(double width, double height, SearchTypeComparison searchType, params long[] idList) {
			var ic = new ResolutionFilterItemCreator(new ComparableSize(width, height), searchType);
			var filter = ic.Create();
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData.Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(idList.Cast<long?>());
		}

		[TestCase(10, null, SearchTypeComparison.GreaterThan, 2, 4, 6)]
		[TestCase(10, null, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 4, 6)]
		[TestCase(11, null, SearchTypeComparison.Equal, 2)]
		[TestCase(10, null, SearchTypeComparison.LessThanOrEqual, 1, 3, 5)]
		[TestCase(10, null, SearchTypeComparison.LessThan, 3, 5)]
		[TestCase(9, null, SearchTypeComparison.GreaterThan, 1, 2, 4, 6)]
		[TestCase(11, null, SearchTypeComparison.GreaterThan, 4, 6)]
		[TestCase(null, 10, SearchTypeComparison.GreaterThan, 3, 5, 6)]
		[TestCase(null, 10, SearchTypeComparison.GreaterThanOrEqual, 1, 3, 5, 6)]
		[TestCase(null, 11, SearchTypeComparison.Equal, 3)]
		[TestCase(null, 10, SearchTypeComparison.LessThanOrEqual, 1, 2, 4)]
		[TestCase(null, 10, SearchTypeComparison.LessThan, 2, 4)]
		[TestCase(null, 9, SearchTypeComparison.GreaterThan, 1, 3, 5, 6)]
		[TestCase(null, 11, SearchTypeComparison.GreaterThan, 5, 6)]
		public void フィルタリング幅高さパターン(int? width, int? height, SearchTypeComparison searchType, params long[] idList) {
			var ic = new ResolutionFilterItemCreator(width, height, searchType);
			var filter = ic.Create();
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}

		[TestCase(10, 10, (SearchTypeInclude)100)]
		public void フィルター作成例外面積パターン(double width, double height, SearchTypeComparison searchType) {
			Assert.Throws<ArgumentException>(() => {
				_ = new ResolutionFilterItemCreator(new ComparableSize(width, height), searchType);
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
				_ = new ResolutionFilterItemCreator(width, height, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
