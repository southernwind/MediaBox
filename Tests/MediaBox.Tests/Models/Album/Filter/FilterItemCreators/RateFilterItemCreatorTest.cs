using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class RateFilterItemCreatorTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();

			this.DataBase.MediaFiles.AddRange(
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, rate: 0),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, rate: 1),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, rate: 2),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image4Png.FilePath, mediaFileId: 4, rate: 3),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, rate: 4),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, rate: 5)
			);
			this.DataBase.SaveChanges();
		}

		[TestCase(0, 1, 2, 3, 4, 5, 6)]
		[TestCase(1, 2, 3, 4, 5, 6)]
		[TestCase(2, 3, 4, 5, 6)]
		[TestCase(3, 4, 5, 6)]
		[TestCase(4, 5, 6)]
		[TestCase(5, 6)]
		public void フィルタリング(int rate, params long[] idList) {
			var ic = new RateFilterItemCreator(rate);
			var filter = ic.Create() as FilterItem;
			this.DataBase.MediaFiles.Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList);
		}
	}
}
