using System;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilteringConditionTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();

			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, rate: 0, width: 30, height: 50, tags: new[] { "aa", "bb" }, latitude: 137.333, longitude: 31.121, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, rate: 1, width: 50, height: 30, tags: new[] { "bb" }, latitude: null, longitude: null, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, rate: 2, width: 10, height: 150, tags: new[] { "cc" }, latitude: 135.123, longitude: 34.121);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, rate: 3, width: 1501, height: 1, latitude: null, longitude: null, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, rate: 4, width: 2, height: 7, latitude: null, longitude: null, altitude: null);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, rate: 5, width: 20, height: 70, tags: new[] { "bb" }, latitude: null, longitude: null, altitude: null);
		}

		[Test]
		public void フィルター作成() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.RestorableFilterObject.Is(rfo);
		}

		[Test]
		public void フィルター名() {
			var rfo = new RestorableFilterObject {
				DisplayName = {
					Value = "FilterName1"
				}
			};
			using var sfc = new FilteringCondition(rfo);
			sfc.DisplayName.Value.Is("FilterName1");
			sfc.DisplayName.Value = "FN2";
			rfo.DisplayName.Value.Is("FN2");
			rfo.DisplayName.Value = "FN3";
			sfc.DisplayName.Value.Is("FN3");
		}

		[Test]
		public void フィルター条件復元() {
			var rfo = new RestorableFilterObject();
			using (var sfc = new FilteringCondition(rfo)) {
				sfc.AddTagFilter("bb", SearchTypeInclude.Exclude);
				sfc.AddFilePathFilter(".jpg", SearchTypeInclude.Include);
			}
			using (var sfc = new FilteringCondition(rfo)) {
				sfc.FilterItemCreators.Count.Is(2);
				var tag = sfc.FilterItemCreators[0].IsInstanceOf<TagFilterItemCreator>();
				tag.TagName.Is("bb");
				tag.SearchType.Is(SearchTypeInclude.Exclude);
				var filepath = sfc.FilterItemCreators[1].IsInstanceOf<FilePathFilterItemCreator>();
				filepath.Text.Is(".jpg");
				filepath.SearchType.Is(SearchTypeInclude.Include);
				sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(3, 5);
			}
		}

		[Test]
		public void フィルターなし() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[TestCase("aa", SearchTypeInclude.Include, 1)]
		[TestCase("bb", SearchTypeInclude.Include, 1, 2, 6)]
		[TestCase("cc", SearchTypeInclude.Include, 3)]
		[TestCase("dd", SearchTypeInclude.Include)]
		[TestCase("aa", SearchTypeInclude.Exclude, 2, 3, 4, 5, 6)]
		[TestCase("bb", SearchTypeInclude.Exclude, 3, 4, 5)]
		[TestCase("cc", SearchTypeInclude.Exclude, 1, 2, 4, 5, 6)]
		[TestCase("dd", SearchTypeInclude.Exclude, 1, 2, 3, 4, 5, 6)]
		public void タグフィルター追加削除(string tag, SearchTypeInclude searchType, params long[] result) {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddTagFilter(tag, searchType);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[TestCase(@"File\image", SearchTypeInclude.Include, 1, 2, 3, 4)]
		[TestCase(@"File\image3", SearchTypeInclude.Include, 3)]
		[TestCase(@"File\vid", SearchTypeInclude.Include, 6)]
		[TestCase(@"File\no_exif", SearchTypeInclude.Include, 5)]
		[TestCase(@"File\image", SearchTypeInclude.Exclude, 5, 6)]
		[TestCase(@"File\image3", SearchTypeInclude.Exclude, 1, 2, 4, 5, 6)]
		[TestCase(@"File\vid", SearchTypeInclude.Exclude, 1, 2, 3, 4, 5)]
		[TestCase(@"File\no_exif", SearchTypeInclude.Exclude, 1, 2, 3, 4, 6)]
		public void ファイルパスフィルター追加削除(string filePath, SearchTypeInclude searchType, params long[] result) {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddFilePathFilter(filePath, searchType);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[TestCase(1, SearchTypeComparison.GreaterThanOrEqual, 2, 3, 4, 5, 6)]
		[TestCase(1, SearchTypeComparison.GreaterThan, 3, 4, 5, 6)]
		[TestCase(2, SearchTypeComparison.GreaterThan, 4, 5, 6)]
		[TestCase(3, SearchTypeComparison.Equal, 4)]
		[TestCase(3, SearchTypeComparison.LessThan, 1, 2, 3)]
		[TestCase(3, SearchTypeComparison.LessThanOrEqual, 1, 2, 3, 4)]
		public void 評価フィルター追加削除(int rate, SearchTypeComparison searchType, params long[] result) {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddRateFilter(rate, searchType);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}


		[TestCase(150, 10, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 3, 4)]
		[TestCase(30, null, SearchTypeComparison.GreaterThan, 2, 4)]
		[TestCase(30, null, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 4)]
		[TestCase(null, 30, SearchTypeComparison.LessThan, 4, 5)]
		[TestCase(null, 30, SearchTypeComparison.LessThanOrEqual, 2, 4, 5)]
		[TestCase(150, 10, SearchTypeComparison.Equal, 1, 2, 3)]
		public void 解像度フィルター追加削除(int? width, int? height, SearchTypeComparison searchType, params long[] result) {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddResolutionFilter(width, height, searchType);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[TestCase(true, 6)]
		[TestCase(false, 1, 2, 3, 4, 5)]
		public void メディアタイプフィルター追加削除(bool isVideo, params long[] result) {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddMediaTypeFilter(isVideo);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[TestCase(true, 1, 3)]
		[TestCase(false, 2, 4, 5, 6)]
		public void 座標フィルター追加削除(bool hasLocation, params long[] result) {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddLocationFilter(hasLocation);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[TestCase(true, 1, 2, 3, 4, 5, 6)]
		[TestCase(false, 7, 8)]
		public void 存在フィルター追加削除(bool exists, params long[] result) {
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NotExistsFileJpg.FilePath, mediaFileId: 7);
			DatabaseUtility.RegisterMediaFileRecord(this.DocumentDb, this.TestFiles.NotExistsFileMov.FilePath, mediaFileId: 8);

			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddExistsFilter(exists);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6, 7, 8);
		}

		[Test]
		public void 複数条件() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddTagFilter("bb", SearchTypeInclude.Include);
			sfc.AddResolutionFilter(150, 10, SearchTypeComparison.GreaterThanOrEqual);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DocumentDb.GetMediaFilesCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void モデルに対するフィルター() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddFilePathFilter(".jpg", SearchTypeInclude.Include);
			var files =
				new[] {
					this.TestFiles.Image1Jpg,
					this.TestFiles.Image2Jpg,
					this.TestFiles.Image3Jpg,
					this.TestFiles.Image4Png,
					this.TestFiles.Image5Bmp,
					this.TestFiles.NotExistsFileJpg,
					this.TestFiles.NotExistsFileMov
				}.Select(x => this.MediaFactory.Create(x.FilePath));

			files.ForEach(x => x.UpdateFileInfo());
			sfc.SetFilterConditions(files).Select(x => x.FilePath).OrderBy(x => x).Is(
				this.TestFiles.Image1Jpg.FilePath,
				this.TestFiles.Image2Jpg.FilePath,
				this.TestFiles.Image3Jpg.FilePath,
				this.TestFiles.NotExistsFileJpg.FilePath);

			sfc.AddExistsFilter(true);
			sfc.SetFilterConditions(files).Select(x => x.FilePath).OrderBy(x => x).Is(
				this.TestFiles.Image1Jpg.FilePath,
				this.TestFiles.Image2Jpg.FilePath,
				this.TestFiles.Image3Jpg.FilePath);
		}

		[Test]
		public void フィルター変更通知() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			var count = 0;
			sfc.OnUpdateFilteringConditions.Subscribe(_ => {
				count++;
			});
			count.Is(0);
			sfc.AddTagFilter("tag", SearchTypeInclude.Exclude);
			count.Is(1);
			sfc.AddRateFilter(4, SearchTypeComparison.GreaterThan);
			count.Is(2);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			count.Is(3);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			count.Is(4);
		}
	}
}
