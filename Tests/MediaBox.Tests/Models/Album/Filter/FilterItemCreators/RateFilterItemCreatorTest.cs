using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class RateFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				DatabaseUtility.CreateMediaFileRecord(1,filePath: this.TestFiles.Image1Jpg.FilePath,rate: 0),
				DatabaseUtility.CreateMediaFileRecord(2,filePath: this.TestFiles.Image2Jpg.FilePath,rate: 1),
				DatabaseUtility.CreateMediaFileRecord(3,filePath: this.TestFiles.Image3Jpg.FilePath,rate: 2),
				DatabaseUtility.CreateMediaFileRecord(4,filePath: this.TestFiles.Image4Png.FilePath,rate: 3),
				DatabaseUtility.CreateMediaFileRecord(5,filePath: this.TestFiles.NoExifJpg.FilePath,rate: 4),
				DatabaseUtility.CreateMediaFileRecord(6,filePath: this.TestFiles.Video1Mov.FilePath,rate: 5)
			};
			this.CreateModels();
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
			var io = new RateFilterItemObject(rate, searchType);
			var ic = new RateFilterItemCreator();
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.AsQueryable().Where(filter.Condition).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
