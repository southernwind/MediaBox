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
			var io = new FilePathFilterItemObject(text, searchType);
			var ic = new FilePathFilterItemCreator();
			var filter = ic.Create(io);
			filter.IncludeSql.Should().Be(true);
			this.TestTableData!.ToLiteDbCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().Should().BeEquivalentTo(idList);
			this.TestModelData!.Where(filter.ConditionForModel).Select(x => x.MediaFileId).Should().BeEquivalentTo(idList);
		}

	}
}
