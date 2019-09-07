using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class FilePathFilterItemCreatorTest : FilterCreatorTestClassBase {
		protected override void SetDatabaseRecord() {
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, @"C:\test\image1.jpg", mediaFileId: 1);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, @"C:\file\image2.png", mediaFileId: 2);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, @"C:\test\image3.jpg", mediaFileId: 3);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, @"D:\test\data\image4.jpg", mediaFileId: 4);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, @"D:\test\file5.jpg", mediaFileId: 5);
		}

		protected override IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(@"C:\test\image1.jpg");
			m1.MediaFileId = 1;
			var m2 = this.MediaFactory.Create(@"C:\file\image2.png");
			m2.MediaFileId = 2;
			var m3 = this.MediaFactory.Create(@"C:\test\image3.jpg");
			m3.MediaFileId = 3;
			var m4 = this.MediaFactory.Create(@"D:\test\data\image4.jpg");
			m4.MediaFileId = 4;
			var m5 = this.MediaFactory.Create(@"D:\test\file5.jpg");
			m5.MediaFileId = 5;

			return new[] { m1, m2, m3, m4, m5 };
		}

		[TestCase("image", SearchTypeInclude.Include, "imageをファイルパスに含む")]
		[TestCase("raccoon", SearchTypeInclude.Exclude, "raccoonをファイルパスに含まない")]
		public void プロパティ(string text, SearchTypeInclude searchType, string displayName) {
			var ic = new FilePathFilterItemCreator(text, searchType);
			ic.Text.Is(text);
			ic.SearchType.Is(searchType);
			ic.DisplayName.Is(displayName);

#pragma warning disable 618
			var ic2 = new FilePathFilterItemCreator();
#pragma warning restore 618
			ic2.Text.IsNull();
			ic2.SearchType.Is(SearchTypeInclude.Include);
			ic2.Text = text;
			ic2.SearchType = searchType;
			ic2.Text.Is(text);
			ic2.SearchType.Is(searchType);
			ic2.DisplayName.Is(displayName);
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
			this.SetDatabaseRecord();
			var ic = new FilePathFilterItemCreator(text, searchType);
			var filter = ic.Create().IsInstanceOf<FilterItem>();
			this.DataBase.MediaFiles.Where(filter.Condition).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList);
			this.CreateModels().Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList.Cast<long?>());
		}

		[TestCase(null, SearchTypeInclude.Include)]
		[TestCase("file", (SearchTypeInclude)100)]
		public void フィルター作成例外(string text, SearchTypeInclude searchType) {
			Assert.Throws<ArgumentException>(() => {
				_ = new FilePathFilterItemCreator(text, searchType);
			});
		}
	}
}
