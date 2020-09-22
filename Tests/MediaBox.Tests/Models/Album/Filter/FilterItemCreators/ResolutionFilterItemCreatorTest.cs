using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

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
			var io = new ResolutionFilterItemObject(new ComparableSize(width, height), searchType);
			var ic = new ResolutionFilterItemCreator();
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.AsQueryable().Where(filter.Condition).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(idList.Cast<long?>());
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
			var io = new ResolutionFilterItemObject(width, height, searchType);
			var ic = new ResolutionFilterItemCreator();
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.AsQueryable().Where(filter.Condition).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
