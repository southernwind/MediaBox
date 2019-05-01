using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class ResolutionFilterItemCreatorTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();

			this.DataBase.MediaFiles.AddRange(
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, width: 10, height: 10),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, width: 11, height: 9),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, width: 9, height: 11),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image4Png.FilePath, mediaFileId: 4, width: 100, height: 1),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, width: 1, height: 100),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, width: 500, height: 3000)
			);
			this.DataBase.SaveChanges();
		}

		[TestCase(10, 10, 1, 4, 5, 6)]
		[TestCase(9, 11, 1, 2, 3, 4, 5, 6)]
		[TestCase(11, 9, 1, 2, 3, 4, 5, 6)]
		[TestCase(1, 100, 1, 4, 5, 6)]
		[TestCase(1, 101, 6)]
		[TestCase(1, 1500000, 6)]
		[TestCase(1, 1500001)]
		public void フィルタリング(double width, double height, params long[] idList) {
			var ic = new ResolutionFilterItemCreator(new ComparableSize(width, height));
			var filter = ic.Create() as FilterItem;
			this.DataBase.MediaFiles.Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList);
		}
	}
}
