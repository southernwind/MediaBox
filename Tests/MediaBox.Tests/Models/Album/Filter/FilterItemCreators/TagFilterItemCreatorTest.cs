using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class TagFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			var tagA = new Tag { TagName = "aaa" };
			var tagB = new Tag { TagName = "bbb" };
			var tagC = new Tag { TagName = "ccc" };
			var tagD = new Tag { TagName = "ddd" };
			var tagE = new Tag { TagName = "eee" };
			this.TestTableData = new[] {
				DatabaseUtility.CreateMediaFileRecord(1,filePath: this.TestFiles.Image1Jpg.FilePath, mediaFileTags: new[] { new MediaFileTag { Tag = tagA }, new MediaFileTag { Tag = tagB } } ),
				DatabaseUtility.CreateMediaFileRecord(2,filePath: this.TestFiles.Image2Jpg.FilePath, mediaFileTags: new[] { new MediaFileTag { Tag = tagA }, new MediaFileTag { Tag = tagB }, new MediaFileTag { Tag = tagC } } ),
				DatabaseUtility.CreateMediaFileRecord(3,filePath: this.TestFiles.Image3Jpg.FilePath, mediaFileTags: new[] { new MediaFileTag { Tag = tagA }, new MediaFileTag { Tag = tagD } } ),
				DatabaseUtility.CreateMediaFileRecord(4,filePath: this.TestFiles.Image4Png.FilePath),
				DatabaseUtility.CreateMediaFileRecord(5,filePath: this.TestFiles.NoExifJpg.FilePath, mediaFileTags: new[] { new MediaFileTag { Tag = tagA }, new MediaFileTag { Tag = tagE } } ),
				DatabaseUtility.CreateMediaFileRecord(6,filePath: this.TestFiles.Video1Mov.FilePath, mediaFileTags: new[] { new MediaFileTag { Tag = tagA }, new MediaFileTag { Tag = tagC }, new MediaFileTag { Tag = tagD } } )
			};
			this.CreateModels();
		}
		[TestCase("aaa", SearchTypeInclude.Include, 1, 2, 3, 5, 6)]
		[TestCase("bbb", SearchTypeInclude.Include, 1, 2)]
		[TestCase("ccc", SearchTypeInclude.Include, 2, 6)]
		[TestCase("ddd", SearchTypeInclude.Include, 3, 6)]
		[TestCase("eee", SearchTypeInclude.Include, 5)]
		[TestCase("fff", SearchTypeInclude.Include)]
		[TestCase("aa", SearchTypeInclude.Include)]
		[TestCase("aaaa", SearchTypeInclude.Include)]
		[TestCase("aaa", SearchTypeInclude.Exclude, 4)]
		[TestCase("bbb", SearchTypeInclude.Exclude, 3, 4, 5, 6)]
		[TestCase("ccc", SearchTypeInclude.Exclude, 1, 3, 4, 5)]
		[TestCase("ddd", SearchTypeInclude.Exclude, 1, 2, 4, 5)]
		[TestCase("eee", SearchTypeInclude.Exclude, 1, 2, 3, 4, 6)]
		[TestCase("fff", SearchTypeInclude.Exclude, 1, 2, 3, 4, 5, 6)]
		[TestCase("aa", SearchTypeInclude.Exclude, 1, 2, 3, 4, 5, 6)]
		[TestCase("aaaa", SearchTypeInclude.Exclude, 1, 2, 3, 4, 5, 6)]
		public void フィルタリング(string tag, SearchTypeInclude searchType, params long[] idList) {
			var io = new TagFilterItemObject(tag, searchType);
			var ic = new TagFilterItemCreator();
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(false);
			this.TestTableData!.AsQueryable().Where(filter.Condition).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
