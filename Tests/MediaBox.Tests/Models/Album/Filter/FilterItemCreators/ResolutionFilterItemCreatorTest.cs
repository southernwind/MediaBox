using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class ResolutionFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				DatabaseUtility.CreateMediaFileRecord(1,filePath: this.TestFiles.Image1Jpg.FilePath, width: 10, height: 10),
				DatabaseUtility.CreateMediaFileRecord(2,filePath: this.TestFiles.Image2Jpg.FilePath, width: 11, height: 9),
				DatabaseUtility.CreateMediaFileRecord(3,filePath: this.TestFiles.Image3Jpg.FilePath, width: 9, height: 11),
				DatabaseUtility.CreateMediaFileRecord(4,filePath: this.TestFiles.Image4Png.FilePath, width: 100, height: 1),
				DatabaseUtility.CreateMediaFileRecord(5,filePath: this.TestFiles.NoExifJpg.FilePath, width: 1, height: 100),
				DatabaseUtility.CreateMediaFileRecord(6,filePath: this.TestFiles.Video1Mov.FilePath, width: 500, height: 3000)
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
