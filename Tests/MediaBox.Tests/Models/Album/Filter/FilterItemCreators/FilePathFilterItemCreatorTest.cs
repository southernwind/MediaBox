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
	internal class FilePathFilterItemCreatorTest : FilterCreatorTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.TestTableData = new[] {
				new MediaFile {MediaFileId = 1, FilePath = @"C:\test\image1.jpg" },
				new MediaFile {MediaFileId = 2, FilePath =  @"C:\file\image2.png"},
				new MediaFile {MediaFileId = 3, FilePath = @"C:\test\image3.jpg"},
				new MediaFile {MediaFileId = 4, FilePath = @"D:\test\data\image4.jpg"},
				new MediaFile {MediaFileId = 5, FilePath = @"D:\test\file5.jpg"},
			};
			this.CreateModels();
		}

		[TestCase("image", SearchTypeInclude.Include, "imageをファイルパスに含む")]
		[TestCase("raccoon", SearchTypeInclude.Exclude, "raccoonをファイルパスに含まない")]
		public void プロパティ(string text, SearchTypeInclude searchType, string displayName) {
			var ic = new FilePathFilterItemCreator(text, searchType);
			ic.Text.Should().Be(text);
			ic.SearchType.Should().Be(searchType);
			ic.DisplayName.Should().Be(displayName);

#pragma warning disable 618
			var ic2 = new FilePathFilterItemCreator();
#pragma warning restore 618
			ic2.Text.Should().BeNull();
			ic2.SearchType.Should().Be(SearchTypeInclude.Include);
			ic2.Text = text;
			ic2.SearchType = searchType;
			ic2.Text.Should().Be(text);
			ic2.SearchType.Should().Be(searchType);
			ic2.DisplayName.Should().Be(displayName);
		}

		[TestCase("image", SearchTypeInclude.Include, 1, 2, 3, 4)]
		[TestCase(@"\test\", SearchTypeInclude.Include, 1, 3, 4, 5)]
		[TestCase(@"C:\", SearchTypeInclude.Include, 1, 2, 3)]
		[TestCase(@"test\data", SearchTypeInclude.Include, 4)]
		[TestCase("file", SearchTypeInclude.Include, 2, 5)]
		[TestCase("image", SearchTypeInclude.Exclude, 5)]
		[TestCase(@"\test\", SearchTypeInclude.Exclude, 2)]
		[TestCase(@"C:\", SearchTypeInclude.Exclude, 4, 5)]
		[TestCase(@"test\data", SearchTypeInclude.Exclude, 1, 2, 3, 5)]
		[TestCase("file", SearchTypeInclude.Exclude, 1, 3, 4)]
		public void フィルタリング(string text, SearchTypeInclude searchType, params long[] idList) {
			var ic = new FilePathFilterItemCreator(text, searchType);
			var filter = ic.Create();
			filter.IncludeSql.Should().Be(true);
			this.TestTableData.ToLiteDbCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().Should().BeEquivalentTo(idList);
			this.TestModelData.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}

		[TestCase(null, SearchTypeInclude.Include)]
		[TestCase("file", (SearchTypeInclude)100)]
		public void フィルター作成例外(string text, SearchTypeInclude searchType) {
			Action act = () => {
				_ = new FilePathFilterItemCreator(text, searchType);
			};
			act.Should().ThrowExactly<ArgumentException>();
		}
	}
}
