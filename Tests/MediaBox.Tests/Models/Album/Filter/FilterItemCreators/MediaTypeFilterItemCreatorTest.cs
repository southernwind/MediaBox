using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class MediaTypeFilterItemCreatorTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();

			this.DataBase.MediaFiles.AddRange(
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image4Png.FilePath, mediaFileId: 4),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Video1Mov.FilePath, mediaFileId: 6)
			);
			this.DataBase.SaveChanges();
		}

		[TestCase(true, 6)]
		[TestCase(false, 1, 2, 3, 4, 5)]
		public void フィルタリングテスト(bool isVideo, params long[] idList) {
			var ic = new MediaTypeFilterItemCreator(isVideo);
			var filter = ic.Create() as FilterItem;
			this.DataBase.MediaFiles.Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList);
		}
	}
}
