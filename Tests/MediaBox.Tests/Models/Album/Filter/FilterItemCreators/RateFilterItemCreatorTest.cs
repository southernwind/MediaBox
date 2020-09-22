using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

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
