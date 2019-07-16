
using System;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class LookupDatabaseAlbumTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();

			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, tags: new[] { "aaa", "bbb" }, subTable: SubTable.Image);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, tags: new[] { "aaa", "bbb", "ccc" }, subTable: SubTable.Image);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, tags: new[] { "aaa", "ddd" }, subTable: SubTable.Image);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, tags: Array.Empty<string>(), subTable: SubTable.Image);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, tags: new[] { "aaa", "eee" }, subTable: SubTable.Image);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, tags: new[] { "aaa", "ccc", "ddd" }, subTable: SubTable.Video);
		}

		[TestCase("aaa", 1, 2, 3, 5, 6)]
		[TestCase("bbb", 1, 2)]
		[TestCase("ccc", 2, 6)]
		[TestCase("ddd", 3, 6)]
		[TestCase("eee", 5)]
		[TestCase("fff")]
		[TestCase("aa")]
		[TestCase("aaaa")]
		public async Task ロードパターン(string tag, params long[] idList) {
			using var selector = new AlbumSelector("main");
			using var la = new LookupDatabaseAlbum(selector);
			la.TagName = tag;
			la.Items.Count.Is(0);
			la.LoadMediaFiles();
			await this.WaitTaskCompleted(3000);
			la.Items.Select(x => x.MediaFileId.Value).Is(idList);
		}
	}
}
