using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class MediaTypeFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				DatabaseUtility.CreateMediaFileRecord(1,filePath: this.TestFiles.Image1Jpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(2,filePath: this.TestFiles.Image2Jpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(3,filePath: this.TestFiles.Image3Jpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(4,filePath: this.TestFiles.Image4Png.FilePath),
				DatabaseUtility.CreateMediaFileRecord(5,filePath: this.TestFiles.NoExifJpg.FilePath),
				DatabaseUtility.CreateMediaFileRecord(6,filePath: this.TestFiles.Video1Mov.FilePath)
			};
			this.CreateModels();
		}

		[TestCase(true, 6)]
		[TestCase(false, 1, 2, 3, 4, 5)]
		public void フィルタリングテスト(bool isVideo, params long[] idList) {
			var io = new MediaTypeFilterItemObject(isVideo);
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			var ic = new MediaTypeFilterItemCreator(settingsMock.Object);
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.AsQueryable().Where(filter.Condition).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
