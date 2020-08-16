using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

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

		[TestCase(true, "動画ファイル")]
		[TestCase(false, "画像ファイル")]
		public void プロパティ(bool isVideo, string displayName) {
			var ic = new MediaTypeFilterItemCreator(isVideo);
			ic.IsVideo.Should().Be(isVideo);
			ic.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var ic2 = new MediaTypeFilterItemCreator();
#pragma warning restore 618
			ic2.IsVideo.Should().Be(false);
			ic2.IsVideo = isVideo;
			ic2.IsVideo.Should().Be(isVideo);
			ic2.DisplayName.Should().Be(displayName);
		}


		[TestCase(true, 6)]
		[TestCase(false, 1, 2, 3, 4, 5)]
		public void フィルタリングテスト(bool isVideo, params long[] idList) {
			var ic = new MediaTypeFilterItemCreator(isVideo);
			var filter = ic.Create() as FilterItem;
			this.TestTableData.ToLiteDbCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().OrderBy(x => x).Should().Equal(idList);
			this.TestModelData.Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(idList.Cast<long?>());
		}
	}
}
