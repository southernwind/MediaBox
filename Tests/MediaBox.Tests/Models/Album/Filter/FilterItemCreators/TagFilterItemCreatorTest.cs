using System;
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
			this.TestTableData = new[] {
				new MediaFile { FilePath = this.TestFiles.Image1Jpg.FilePath, MediaFileId= 1, Tags= new[] { "aaa", "bbb" } },
				new MediaFile { FilePath = this.TestFiles.Image2Jpg.FilePath, MediaFileId= 2, Tags= new[] { "aaa", "bbb", "ccc" } },
				new MediaFile { FilePath = this.TestFiles.Image3Jpg.FilePath, MediaFileId= 3, Tags= new[] { "aaa", "ddd" } },
				new MediaFile { FilePath = this.TestFiles.Image4Png.FilePath, MediaFileId= 4, Tags= Array.Empty<string>() },
				new MediaFile { FilePath = this.TestFiles.NoExifJpg.FilePath, MediaFileId= 5, Tags= new[] { "aaa", "eee" } },
				new MediaFile { FilePath = this.TestFiles.Video1Mov.FilePath, MediaFileId= 6, Tags= new[] { "aaa", "ccc", "ddd" } }
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
			this.TestTableData!.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}
	}
}
