using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter.FilterItemCreators {
	internal class ResolutionFilterItemCreatorTest : FilterCreatorTestClassBase {
		protected override void SetDatabaseRecord() {
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, width: 10, height: 10);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, width: 11, height: 9);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, width: 9, height: 11);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, width: 100, height: 1);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, width: 1, height: 100);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, width: 500, height: 3000);
		}

		protected override IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			m1.MediaFileId = 1;
			m1.Resolution = new ComparableSize(10, 10);
			var m2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			m2.MediaFileId = 2;
			m2.Resolution = new ComparableSize(11, 9);
			var m3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			m3.MediaFileId = 3;
			m3.Resolution = new ComparableSize(9, 11);
			var m4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			m4.MediaFileId = 4;
			m4.Resolution = new ComparableSize(100, 1);
			var m5 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			m5.MediaFileId = 5;
			m5.Resolution = new ComparableSize(1, 100);
			var m6 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);
			m6.MediaFileId = 6;
			m6.Resolution = new ComparableSize(500, 3000);

			return new[] { m1, m2, m3, m4, m5, m6 };
		}

		[TestCase(10, 10, SearchTypeComparison.GreaterThan, "解像度が10x10を超える")]
		[TestCase(20, 10, SearchTypeComparison.GreaterThanOrEqual, "解像度が20x10以上")]
		[TestCase(10, 30, SearchTypeComparison.Equal, "解像度が10x30と等しい")]
		[TestCase(40, 10, SearchTypeComparison.LessThanOrEqual, "解像度が40x10以下")]
		[TestCase(10, 50, SearchTypeComparison.LessThan, "解像度が10x50未満")]
		public void プロパティ面積パターン(double width, double height, SearchTypeComparison searchType, string displayName) {
			var ic = new ResolutionFilterItemCreator(new ComparableSize(width, height), searchType);
			ic.Width.IsNull();
			ic.Height.IsNull();
			ic.Resolution.Is(new ComparableSize(width, height));
			ic.SearchType.Is(searchType);
			ic.DisplayName.Is(displayName);

#pragma warning disable 618
			var ic2 = new ResolutionFilterItemCreator();
#pragma warning restore 618
			ic2.Width.IsNull();
			ic2.Height.IsNull();
			ic2.Resolution.IsNull();
			ic2.SearchType.Is(SearchTypeComparison.GreaterThan);
			ic2.Resolution = new ComparableSize(width, height);
			ic2.SearchType = searchType;
			ic2.Resolution.Is(new ComparableSize(width, height));
			ic2.SearchType.Is(searchType);
			ic2.DisplayName.Is(displayName);
		}

		[TestCase(10, null, SearchTypeComparison.GreaterThan, "幅が10を超える")]
		[TestCase(20, null, SearchTypeComparison.GreaterThanOrEqual, "幅が20以上")]
		[TestCase(30, null, SearchTypeComparison.Equal, "幅が30と等しい")]
		[TestCase(40, null, SearchTypeComparison.LessThanOrEqual, "幅が40以下")]
		[TestCase(50, null, SearchTypeComparison.LessThan, "幅が50未満")]
		[TestCase(null, 10, SearchTypeComparison.GreaterThan, "高さが10を超える")]
		[TestCase(null, 20, SearchTypeComparison.GreaterThanOrEqual, "高さが20以上")]
		[TestCase(null, 30, SearchTypeComparison.Equal, "高さが30と等しい")]
		[TestCase(null, 40, SearchTypeComparison.LessThanOrEqual, "高さが40以下")]
		[TestCase(null, 50, SearchTypeComparison.LessThan, "高さが50未満")]
		public void プロパティ幅高さパターン(int? width, int? height, SearchTypeComparison searchType, string displayName) {
			var ic = new ResolutionFilterItemCreator(width, height, searchType);
			ic.Width.Is(width);
			ic.Height.Is(height);
			ic.Resolution.IsNull();
			ic.SearchType.Is(searchType);
			ic.DisplayName.Is(displayName);

#pragma warning disable 618
			var ic2 = new ResolutionFilterItemCreator();
#pragma warning restore 618
			ic2.Width.IsNull();
			ic2.Height.IsNull();
			ic2.Resolution.IsNull();
			Assert.Throws<InvalidOperationException>(() => {
				_ = ic2.DisplayName;
			});
			ic2.SearchType.Is(SearchTypeComparison.GreaterThan);
			ic2.Width = width;
			ic2.Height = height;
			ic2.SearchType = searchType;
			ic2.Width.Is(width);
			ic2.Height.Is(height);
			ic2.SearchType.Is(searchType);
			ic2.DisplayName.Is(displayName);
		}

		[TestCase(10, 10, SearchTypeComparison.GreaterThan, 6)]
		[TestCase(10, 10, SearchTypeComparison.GreaterThanOrEqual, 1, 4, 5, 6)]
		[TestCase(10, 10, SearchTypeComparison.Equal, 1, 4, 5)]
		[TestCase(10, 10, SearchTypeComparison.LessThanOrEqual, 1, 2, 3, 4, 5)]
		[TestCase(10, 10, SearchTypeComparison.LessThan, 2, 3)]
		[TestCase(9, 11, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 3, 4, 5, 6)]
		[TestCase(11, 9, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 3, 4, 5, 6)]
		[TestCase(1, 100, SearchTypeComparison.GreaterThanOrEqual, 1, 4, 5, 6)]
		[TestCase(1, 101, SearchTypeComparison.GreaterThanOrEqual, 6)]
		[TestCase(1, 1500000, SearchTypeComparison.GreaterThanOrEqual, 6)]
		[TestCase(1, 1500001, SearchTypeComparison.GreaterThanOrEqual)]
		public void フィルタリング面積パターン(double width, double height, SearchTypeComparison searchType, params long[] idList) {
			this.SetDatabaseRecord();
			var ic = new ResolutionFilterItemCreator(new ComparableSize(width, height), searchType);
			var filter = ic.Create() as FilterItem;
			this.DocumentDb.GetMediaFilesCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().OrderBy(x => x).Is(idList);
			this.CreateModels().Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList.Cast<long?>());
		}

		[TestCase(10, null, SearchTypeComparison.GreaterThan, 2, 4, 6)]
		[TestCase(10, null, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 4, 6)]
		[TestCase(11, null, SearchTypeComparison.Equal, 2)]
		[TestCase(10, null, SearchTypeComparison.LessThanOrEqual, 1, 3, 5)]
		[TestCase(10, null, SearchTypeComparison.LessThan, 3, 5)]
		[TestCase(9, null, SearchTypeComparison.GreaterThan, 1, 2, 4, 6)]
		[TestCase(11, null, SearchTypeComparison.GreaterThan, 4, 6)]
		[TestCase(null, 10, SearchTypeComparison.GreaterThan, 3, 5, 6)]
		[TestCase(null, 10, SearchTypeComparison.GreaterThanOrEqual, 1, 3, 5, 6)]
		[TestCase(null, 11, SearchTypeComparison.Equal, 3)]
		[TestCase(null, 10, SearchTypeComparison.LessThanOrEqual, 1, 2, 4)]
		[TestCase(null, 10, SearchTypeComparison.LessThan, 2, 4)]
		[TestCase(null, 9, SearchTypeComparison.GreaterThan, 1, 3, 5, 6)]
		[TestCase(null, 11, SearchTypeComparison.GreaterThan, 5, 6)]
		public void フィルタリング幅高さパターン(int? width, int? height, SearchTypeComparison searchType, params long[] idList) {
			this.SetDatabaseRecord();
			var ic = new ResolutionFilterItemCreator(width, height, searchType);
			var filter = ic.Create() as FilterItem;
			this.DocumentDb.GetMediaFilesCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().OrderBy(x => x).Is(idList);
			this.CreateModels().Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList.Cast<long?>());
		}

		[TestCase(10, 10, (SearchTypeInclude)100)]
		public void フィルター作成例外面積パターン(double width, double height, SearchTypeComparison searchType) {
			Assert.Throws<ArgumentException>(() => {
				_ = new ResolutionFilterItemCreator(new ComparableSize(width, height), searchType);
			});
		}

		[TestCase(null, null, SearchTypeComparison.GreaterThan)]
		[TestCase(null, null, SearchTypeComparison.GreaterThanOrEqual)]
		[TestCase(null, null, SearchTypeComparison.Equal)]
		[TestCase(null, null, SearchTypeComparison.LessThanOrEqual)]
		[TestCase(null, null, SearchTypeComparison.LessThan)]
		[TestCase(10, 10, SearchTypeComparison.GreaterThan)]
		[TestCase(10, 10, SearchTypeComparison.GreaterThanOrEqual)]
		[TestCase(10, 10, SearchTypeComparison.Equal)]
		[TestCase(10, 10, SearchTypeComparison.LessThanOrEqual)]
		[TestCase(10, 10, SearchTypeComparison.LessThan)]
		[TestCase(10, null, (SearchTypeComparison)100)]
		public void フィルター作成例外幅高さパターン(int? width, int? height, SearchTypeComparison searchType) {
			Assert.Throws<ArgumentException>(() => {
				_ = new ResolutionFilterItemCreator(width, height, searchType);
			});
		}
	}
}
