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
	internal class RateFilterItemCreatorTest : FilterCreatorTestClassBase {
		protected override void SetDatabaseRecord() {
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, rate: 0);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, rate: 1);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, rate: 2);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, rate: 3);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, rate: 4);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, rate: 5);
		}

		protected override IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			m1.MediaFileId = 1;
			m1.Rate = 0;
			var m2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			m2.MediaFileId = 2;
			m2.Rate = 1;
			var m3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			m3.MediaFileId = 3;
			m3.Rate = 2;
			var m4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			m4.MediaFileId = 4;
			m4.Rate = 3;
			var m5 = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			m5.MediaFileId = 5;
			m5.Rate = 4;
			var m6 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);
			m6.MediaFileId = 6;
			m6.Rate = 5;

			return new[] { m1, m2, m3, m4, m5, m6 };
		}

		[TestCase(1, SearchTypeComparison.GreaterThan, "評価が1を超える")]
		[TestCase(2, SearchTypeComparison.GreaterThanOrEqual, "評価が2以上")]
		[TestCase(3, SearchTypeComparison.Equal, "評価が3と等しい")]
		[TestCase(4, SearchTypeComparison.LessThanOrEqual, "評価が4以下")]
		[TestCase(5, SearchTypeComparison.LessThan, "評価が5未満")]
		public void プロパティ(int rate, SearchTypeComparison searchType, string displayName) {
			var ic = new RateFilterItemCreator(rate, searchType);
			ic.Rate.Is(rate);
			ic.SearchType.Is(searchType);
			ic.DisplayName.Is(displayName);

#pragma warning disable 618
			var ic2 = new RateFilterItemCreator();
#pragma warning restore 618
			ic2.Rate.Is(0);
			ic2.SearchType.Is(SearchTypeComparison.GreaterThan);
			ic2.Rate = rate;
			ic2.SearchType = searchType;
			ic2.Rate.Is(rate);
			ic2.SearchType.Is(searchType);
			ic2.DisplayName.Is(displayName);
		}

		[TestCase(3, SearchTypeComparison.GreaterThan, 5, 6)]
		[TestCase(3, SearchTypeComparison.GreaterThanOrEqual, 4, 5, 6)]
		[TestCase(3, SearchTypeComparison.Equal, 4)]
		[TestCase(3, SearchTypeComparison.LessThanOrEqual, 1, 2, 3, 4)]
		[TestCase(3, SearchTypeComparison.LessThan, 1, 2, 3)]
		[TestCase(1, SearchTypeComparison.GreaterThanOrEqual, 2, 3, 4, 5, 6)]
		[TestCase(2, SearchTypeComparison.GreaterThanOrEqual, 3, 4, 5, 6)]
		[TestCase(3, SearchTypeComparison.GreaterThanOrEqual, 4, 5, 6)]
		[TestCase(4, SearchTypeComparison.GreaterThanOrEqual, 5, 6)]
		[TestCase(5, SearchTypeComparison.GreaterThanOrEqual, 6)]
		public void フィルタリング(int rate, SearchTypeComparison searchType, params long[] idList) {
			this.SetDatabaseRecord();
			var ic = new RateFilterItemCreator(rate, searchType);
			var filter = ic.Create() as FilterItem;
			this.DocumentDb.GetMediaFilesCollection().Query().Where(filter.Condition).Select(x => x.MediaFileId).ToEnumerable().OrderBy(x => x).Is(idList);
			this.CreateModels().Where(filter.ConditionForModel).Select(x => x.MediaFileId).OrderBy(x => x).Is(idList.Cast<long?>());
		}

		[TestCase(5, (SearchTypeComparison)100)]
		public void フィルター作成例外(int rate, SearchTypeComparison searchType) {
			Assert.Throws<ArgumentException>(() => {
				_ = new RateFilterItemCreator(rate, searchType);
			});
		}
	}
}
