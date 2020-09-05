using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilteringConditionTest : ModelTestClassBase {
		private List<MediaFile> _testData = null!;
		public override void SetUp() {
			base.SetUp();
			this._testData = new() {
				new MediaFile { FilePath = this.TestFiles.Image1Jpg.FilePath, MediaFileId = 1, Rate = 0, Width = 30, Height = 50, Tags = new[] { "aa", "bb" }, Latitude = 137.333, Longitude = 31.121, Altitude = null },
				new MediaFile { FilePath = this.TestFiles.Image2Jpg.FilePath, MediaFileId = 2, Rate = 1, Width = 50, Height = 30, Tags = new[] { "bb" }, Latitude = null, Longitude = null, Altitude = null },
				new MediaFile { FilePath = this.TestFiles.Image3Jpg.FilePath, MediaFileId = 3, Rate = 2, Width = 10, Height = 150, Tags = new[] { "cc" }, Latitude = 135.123, Longitude = 34.121 },
				new MediaFile { FilePath = this.TestFiles.Image4Png.FilePath, MediaFileId = 4, Rate = 3, Width = 1501, Height = 1, Latitude = null, Longitude = null, Altitude = null },
				new MediaFile { FilePath = this.TestFiles.NoExifJpg.FilePath, MediaFileId = 5, Rate = 4, Width = 2, Height = 7, Latitude = null, Longitude = null, Altitude = null },
				new MediaFile { FilePath = this.TestFiles.Video1Mov.FilePath, MediaFileId = 6, Rate = 5, Width = 20, Height = 70, Tags = new[] { "bb" }, Latitude = null, Longitude = null, Altitude = null }
			};
		}

		[Test]
		public void フィルター作成() {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.RestorableFilterObject.Should().Be(rfo);
		}

		[Test]
		public void フィルター名() {
			var rfo = new RestorableFilterObject {
				DisplayName = {
					Value = "FilterName1"
				}
			};
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.DisplayName.Value.Should().Be("FilterName1");
			sfc.DisplayName.Value = "FN2";
			rfo.DisplayName.Value.Should().Be("FN2");
			rfo.DisplayName.Value = "FN3";
			sfc.DisplayName.Value.Should().Be("FN3");
		}

		[Test]
		public void フィルター条件復元() {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using (var sfc = new FilteringCondition(rfo, settingsMock.Object)) {
				sfc.AddTagFilter("bb", SearchTypeInclude.Exclude);
				sfc.AddFilePathFilter(".jpg", SearchTypeInclude.Include);
			}
			using (var sfc = new FilteringCondition(rfo, settingsMock.Object)) {
				sfc.FilterItemCreators.Count.Should().Be(2);
				var tag = sfc.FilterItemCreators[0].As<TagFilterItemCreator>();
				tag.TagName.Should().Be("bb");
				tag.SearchType.Should().Be(SearchTypeInclude.Exclude);
				var filepath = sfc.FilterItemCreators[1].As<FilePathFilterItemCreator>();
				filepath.Text.Should().Be(".jpg");
				filepath.SearchType.Should().Be(SearchTypeInclude.Include);
				sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 3, 5 });
			}
		}

		[Test]
		public void フィルターなし() {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
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
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddTagFilter(tag, searchType);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
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
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddFilePathFilter(filePath, searchType);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(1, SearchTypeComparison.GreaterThanOrEqual, 2, 3, 4, 5, 6)]
		[TestCase(1, SearchTypeComparison.GreaterThan, 3, 4, 5, 6)]
		[TestCase(2, SearchTypeComparison.GreaterThan, 4, 5, 6)]
		[TestCase(3, SearchTypeComparison.Equal, 4)]
		[TestCase(3, SearchTypeComparison.LessThan, 1, 2, 3)]
		[TestCase(3, SearchTypeComparison.LessThanOrEqual, 1, 2, 3, 4)]
		public void 評価フィルター追加削除(int rate, SearchTypeComparison searchType, params long[] result) {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddRateFilter(rate, searchType);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}


		[TestCase(150, 10, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 3, 4)]
		[TestCase(30, null, SearchTypeComparison.GreaterThan, 2, 4)]
		[TestCase(30, null, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 4)]
		[TestCase(null, 30, SearchTypeComparison.LessThan, 4, 5)]
		[TestCase(null, 30, SearchTypeComparison.LessThanOrEqual, 2, 4, 5)]
		[TestCase(150, 10, SearchTypeComparison.Equal, 1, 2, 3)]
		public void 解像度フィルター追加削除(int? width, int? height, SearchTypeComparison searchType, params long[] result) {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddResolutionFilter(width, height, searchType);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(true, 6)]
		[TestCase(false, 1, 2, 3, 4, 5)]
		public void メディアタイプフィルター追加削除(bool isVideo, params long[] result) {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddMediaTypeFilter(isVideo);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(true, 1, 3)]
		[TestCase(false, 2, 4, 5, 6)]
		public void 座標フィルター追加削除(bool hasLocation, params long[] result) {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddLocationFilter(hasLocation);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(true, 1, 2, 3, 4, 5, 6)]
		[TestCase(false, 7, 8)]
		public void 存在フィルター追加削除(bool exists, params long[] result) {
			this._testData.AddRange(new[]{
				new MediaFile { FilePath = this.TestFiles.NotExistsFileJpg.FilePath, MediaFileId = 7 },
				new MediaFile { FilePath =this.TestFiles.NotExistsFileMov.FilePath, MediaFileId =8 }
			});

			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddExistsFilter(exists);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6, 7, 8 });
		}

		[Test]
		public void 複数条件() {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddTagFilter("bb", SearchTypeInclude.Include);
			sfc.AddResolutionFilter(150, 10, SearchTypeComparison.GreaterThanOrEqual);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2 });
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this._testData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[Test]
		public void モデルに対するフィルター() {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			sfc.AddFilePathFilter(".jpg", SearchTypeInclude.Include);
			this._testData.AddRange(new[]{
				new MediaFile { FilePath = this.TestFiles.NotExistsFileJpg.FilePath, MediaFileId = 7 },
				new MediaFile { FilePath =this.TestFiles.NotExistsFileMov.FilePath, MediaFileId =8 }
			});
			var files = this._testData.Select(x => x.ToModel());

			files.ForEach(x => x.UpdateFileInfo());
			sfc.SetFilterConditions(files).Select(x => x.FilePath).Should().BeEquivalentTo(
				new[] {
				this.TestFiles.Image1Jpg.FilePath,
				this.TestFiles.Image2Jpg.FilePath,
				this.TestFiles.Image3Jpg.FilePath,
				this.TestFiles.NoExifJpg.FilePath,
				this.TestFiles.NotExistsFileJpg.FilePath
			});

			sfc.AddExistsFilter(true);
			sfc.SetFilterConditions(files).Select(x => x.FilePath).Should().BeEquivalentTo(
				new[] {
				this.TestFiles.Image1Jpg.FilePath,
				this.TestFiles.Image2Jpg.FilePath,
				this.TestFiles.Image3Jpg.FilePath,
				this.TestFiles.NoExifJpg.FilePath
			});
		}

		[Test]
		public void フィルター変更通知() {
			var rfo = new RestorableFilterObject();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var sfc = new FilteringCondition(rfo, settingsMock.Object);
			var count = 0;
			sfc.OnUpdateFilteringConditions.Subscribe(_ => {
				count++;
			});
			count.Should().Be(0);
			sfc.AddTagFilter("tag", SearchTypeInclude.Exclude);
			count.Should().Be(1);
			sfc.AddRateFilter(4, SearchTypeComparison.GreaterThan);
			count.Should().Be(2);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			count.Should().Be(3);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			count.Should().Be(4);
		}
	}
}
