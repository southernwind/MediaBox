using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class TagFilterItemCreatorTest : FilterCreatorTestClassBase {
		protected override void SetDatabaseRecord() {
			base.SetUp();

			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, tags: new[] { "aaa", "bbb" });
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, tags: new[] { "aaa", "bbb", "ccc" });
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, tags: new[] { "aaa", "ddd" });
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, tags: Array.Empty<string>());
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, tags: new[] { "aaa", "eee" });
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, tags: new[] { "aaa", "ccc", "ddd" });

		}

		protected override IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			m1.MediaFileId = 1;
			m1.Tags.AddRange("aaa", "bbb");
			var m2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			m2.MediaFileId = 2;
			m2.Tags.AddRange("aaa", "bbb", "ccc");
			var m3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			m3.MediaFileId = 3;
			m3.Tags.AddRange("aaa", "ddd");
			var m4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			m4.MediaFileId = 4;
			m4.Tags.AddRange();
			var m5 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			m5.MediaFileId = 5;
			m5.Tags.AddRange("aaa", "eee");
			var m6 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);
			m6.MediaFileId = 6;
			m6.Tags.AddRange("aaa", "ccc", "ddd");

			return new[] { m1, m2, m3, m4, m5, m6 };
		}

		[TestCase("tag", SearchTypeInclude.Include, "tagをタグに含む")]
		[TestCase("raccoon", SearchTypeInclude.Exclude, "raccoonをタグに含まない")]
		public void プロパティ(string tag, SearchTypeInclude searchType, string displayName) {
			var ic = new TagFilterItemCreator(tag, searchType);
			ic.TagName.Is(tag);
			ic.SearchType.Is(searchType);
			ic.DisplayName.Is(displayName);

#pragma warning disable 618
			var ic2 = new TagFilterItemCreator();
#pragma warning restore 618
			ic2.TagName.IsNull();
			ic2.SearchType.Is(SearchTypeInclude.Include);
			ic2.TagName = tag;
			ic2.SearchType = searchType;
			ic2.TagName.Is(tag);
			ic2.SearchType.Is(searchType);
			ic2.DisplayName.Is(displayName);
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
			this.SetDatabaseRecord();
			var ic = new TagFilterItemCreator(tag, searchType);
			var filter = ic.Create() as FilterItem;
			this.DocumentDb.GetMediaFilesCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().OrderBy(x => x).Is(idList);
			this.CreateModels().Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList.Cast<long?>());
		}

		[TestCase(null, SearchTypeInclude.Include)]
		[TestCase("tag", (SearchTypeInclude)100)]
		public void フィルター作成例外(string tag, SearchTypeInclude searchType) {
			Assert.Throws<ArgumentException>(() => {
				_ = new TagFilterItemCreator(tag, searchType);
			});
		}
	}
}
