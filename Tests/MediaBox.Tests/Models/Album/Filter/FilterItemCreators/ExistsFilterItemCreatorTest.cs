using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class ExistsItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				DatabaseUtility.CreateMediaFileRecord(1, filePath: this.TestFiles.Image1Jpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(2, filePath: this.TestFiles.Image2Jpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(3, filePath: this.TestFiles.Image3Jpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(4, filePath: this.TestFiles.Image4Png.FilePath),
				DatabaseUtility.CreateMediaFileRecord(5, filePath: this.TestFiles.NotExistsFileJpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(6, filePath: this.TestFiles.NotExistsFileMov.FilePath)
			};
			this.CreateModels();
		}

		[TestCase(true, 1, 2, 3, 4)]
		[TestCase(false, 5, 6)]
		public void フィルタリングテスト(bool exists, params long[] idList) {
			var io = new ExistsFilterItemObject(exists);
			var ic = new ExistsFilterItemCreator();
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.AsQueryable().Where(filter.Condition).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
