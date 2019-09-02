using System;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class TagFilterItemCreatorTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();

			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, tags: new[] { "aaa", "bbb" });
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, tags: new[] { "aaa", "bbb", "ccc" });
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, tags: new[] { "aaa", "ddd" });
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, tags: Array.Empty<string>());
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, tags: new[] { "aaa", "eee" });
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, tags: new[] { "aaa", "ccc", "ddd" });

		}

		[TestCase("aaa", 1, 2, 3, 5, 6)]
		[TestCase("bbb", 1, 2)]
		[TestCase("ccc", 2, 6)]
		[TestCase("ddd", 3, 6)]
		[TestCase("eee", 5)]
		[TestCase("fff")]
		[TestCase("aa")]
		[TestCase("aaaa")]
		public void フィルタリング(string tag, params long[] idList) {
			var ic = new TagFilterItemCreator(tag, SearchTypeInclude.Include);
			var filter = ic.Create() as FilterItem;
			this.DataBase.MediaFiles.Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList);
		}
	}
}
