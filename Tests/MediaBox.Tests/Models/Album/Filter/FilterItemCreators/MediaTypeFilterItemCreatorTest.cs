using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
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
				new MediaFile{FilePath= this.TestFiles.Image1Jpg.FilePath, MediaFileId= 1},
				new MediaFile{FilePath= this.TestFiles.Image2Jpg.FilePath, MediaFileId= 2},
				new MediaFile{FilePath= this.TestFiles.Image3Jpg.FilePath, MediaFileId= 3},
				new MediaFile{FilePath= this.TestFiles.Image4Png.FilePath, MediaFileId= 4},
				new MediaFile{FilePath= this.TestFiles.NoExifJpg.FilePath, MediaFileId= 5},
				new MediaFile{FilePath= this.TestFiles.Video1Mov.FilePath, MediaFileId= 6}
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
			this.TestTableData!.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
