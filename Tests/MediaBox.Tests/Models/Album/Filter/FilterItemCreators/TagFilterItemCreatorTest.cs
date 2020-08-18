using System;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
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

		[TestCase("tag", SearchTypeInclude.Include, "tagをタグに含む")]
		[TestCase("raccoon", SearchTypeInclude.Exclude, "raccoonをタグに含まない")]
		public void プロパティ(string tag, SearchTypeInclude searchType, string displayName) {
			var ic = new TagFilterItemCreator(tag, searchType);
			ic.TagName.Should().Be(tag);
			ic.SearchType.Should().Be(searchType);
			ic.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var ic2 = new TagFilterItemCreator();
#pragma warning restore 618
			ic2.TagName.Should().BeNull();
			ic2.SearchType.Should().Be(SearchTypeInclude.Include);
			ic2.TagName = tag;
			ic2.SearchType = searchType;
			ic2.TagName.Should().Be(tag);
			ic2.SearchType.Should().Be(searchType);
			ic2.DisplayName.Should().Be(displayName);
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
			var ic = new TagFilterItemCreator(tag, searchType);
			var filter = ic.Create() as FilterItem;
			filter.IncludeSql.Should().Be(false);
			this.TestTableData.ToLiteDbCollection().Query().ToEnumerable().Where(filter.Condition.Compile()).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
			this.TestModelData.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}

		[TestCase(null, SearchTypeInclude.Include)]
		[TestCase("tag", (SearchTypeInclude)100)]
		public void フィルター作成例外(string tag, SearchTypeInclude searchType) {
			Action act = () => {
				_ = new TagFilterItemCreator(tag, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
