using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
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

		[TestCase(true, "ファイルが存在する")]
		[TestCase(false, "ファイルが存在しない")]
		public void プロパティ(bool contains, string displayName) {
			var ic = new ExistsFilterItemCreator(contains);
			ic.Exists.Should().Be(contains);
			ic.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var ic2 = new ExistsFilterItemCreator();
#pragma warning restore 618
			ic2.Exists.Should().Be(false);
			ic2.Exists = contains;
			ic2.Exists.Should().Be(contains);
			ic2.DisplayName.Should().Be(displayName);
		}


		[TestCase(true, 1, 2, 3, 4)]
		[TestCase(false, 5, 6)]
		public void フィルタリングテスト(bool exists, params long[] idList) {
			var ic = new ExistsFilterItemCreator(exists);
			var filter = ic.Create();
			filter.IncludeSql.Should().Be(false);
			this.TestTableData.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
