using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class RateFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				new MediaFile { FilePath = this.TestFiles.Image1Jpg.FilePath, MediaFileId = 1, Rate = 0 },
				new MediaFile { FilePath = this.TestFiles.Image2Jpg.FilePath, MediaFileId = 2, Rate = 1 },
				new MediaFile { FilePath = this.TestFiles.Image3Jpg.FilePath, MediaFileId = 3, Rate = 2 },
				new MediaFile { FilePath = this.TestFiles.Image4Png.FilePath, MediaFileId = 4, Rate = 3 },
				new MediaFile { FilePath = this.TestFiles.NoExifJpg.FilePath, MediaFileId = 5, Rate = 4 },
				new MediaFile { FilePath = this.TestFiles.Video1Mov.FilePath, MediaFileId = 6, Rate = 5 }
			};
			this.CreateModels();
		}

		[TestCase(1, SearchTypeComparison.GreaterThan, "評価が1を超える")]
		[TestCase(2, SearchTypeComparison.GreaterThanOrEqual, "評価が2以上")]
		[TestCase(3, SearchTypeComparison.Equal, "評価が3と等しい")]
		[TestCase(4, SearchTypeComparison.LessThanOrEqual, "評価が4以下")]
		[TestCase(5, SearchTypeComparison.LessThan, "評価が5未満")]
		public void プロパティ(int rate, SearchTypeComparison searchType, string displayName) {
			var ic = new RateFilterItemCreator(rate, searchType);
			ic.Rate.Should().Be(rate);
			ic.SearchType.Should().Be(searchType);
			ic.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var ic2 = new RateFilterItemCreator();
#pragma warning restore 618
			ic2.Rate.Should().Be(0);
			ic2.SearchType.Should().Be(SearchTypeComparison.GreaterThan);
			ic2.Rate = rate;
			ic2.SearchType = searchType;
			ic2.Rate.Should().Be(rate);
			ic2.SearchType.Should().Be(searchType);
			ic2.DisplayName.Should().Be(displayName);
		}

		[TestCase(3, SearchTypeComparison.GreaterThan, 5, 6)]
		[TestCase(3, SearchTypeComparison.GreaterThanOrEqual, 4, 5, 6)]
		[TestCase(3, SearchTypeComparison.Equal, 4)]
		[TestCase(3, SearchTypeComparison.LessThanOrEqual, 1, 2, 3, 4)]
		[TestCase(3, SearchTypeComparison.LessThan, 1, 2, 3)]
		[TestCase(1, SearchTypeComparison.GreaterThanOrEqual, 2, 3, 4, 5, 6)]
		[TestCase(2, SearchTypeComparison.GreaterThanOrEqual, 3, 4, 5, 6)]
		[TestCase(3, SearchTypeComparison.GreaterThanOrEqual, 4, 5, 6)]
		[TestCase(4, SearchTypeComparison.GreaterThanOrEqual, 5, 6)]
		[TestCase(5, SearchTypeComparison.GreaterThanOrEqual, 6)]
		public void フィルタリング(int rate, SearchTypeComparison searchType, params long[] idList) {
			var ic = new RateFilterItemCreator(rate, searchType);
			var filter = ic.Create();
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}

		[TestCase(5, (SearchTypeComparison)100)]
		public void フィルター作成例外(int rate, SearchTypeComparison searchType) {
			Action act = () => {
				_ = new RateFilterItemCreator(rate, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
