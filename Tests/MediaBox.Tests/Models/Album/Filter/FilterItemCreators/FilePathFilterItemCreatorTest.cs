using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class FilePathFilterItemCreatorTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();

			this.DataBase.MediaFiles.AddRange(
				DatabaseUtility.GetMediaFileRecord(@"C:\test\image1.jpg", mediaFileId: 1),
				DatabaseUtility.GetMediaFileRecord(@"C:\file\image2.png", mediaFileId: 2),
				DatabaseUtility.GetMediaFileRecord(@"C:\test\image3.jpg", mediaFileId: 3),
				DatabaseUtility.GetMediaFileRecord(@"D:\test\data\image4.jpg", mediaFileId: 4),
				DatabaseUtility.GetMediaFileRecord(@"D:\test\file5.jpg", mediaFileId: 5)
			);
			this.DataBase.SaveChanges();
		}

		[TestCase(null)]
		[TestCase("image", 1, 2, 3, 4)]
		[TestCase(@"\test\", 1, 3, 4, 5)]
		[TestCase(@"C:\", 1, 2, 3)]
		[TestCase(@"test\data", 4)]
		[TestCase("file", 2, 5)]
		public void フィルタリング(string text, params long[] idList) {
			var ic = new FilePathFilterItemCreator(text);
			var filter = ic.Create() as FilterItem;
			this.DataBase.MediaFiles.Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList);
		}
	}
}
