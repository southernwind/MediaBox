using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class ExistsItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				new MediaFile {MediaFileId = 1, FilePath = this.TestFiles.Image1Jpg.FilePath },
				new MediaFile {MediaFileId = 2, FilePath = this.TestFiles.Image2Jpg.FilePath},
				new MediaFile {MediaFileId = 3, FilePath = this.TestFiles.Image3Jpg.FilePath},
				new MediaFile {MediaFileId = 4, FilePath = this.TestFiles.Image4Png.FilePath},
				new MediaFile {MediaFileId = 5, FilePath = this.TestFiles.NotExistsFileJpg.FilePath},
				new MediaFile {MediaFileId = 6, FilePath = this.TestFiles.NotExistsFileMov.FilePath}
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
			this.TestTableData!.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
