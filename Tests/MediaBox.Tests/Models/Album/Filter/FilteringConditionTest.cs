using System;
using System.Collections.Generic;
using System.Linq;

using FluentAssertions;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilteringConditionTest : ModelTestClassBase {
		private List<MediaFile> _testData = null!;
		private DbContextMockCreator _dbContextMockCreator = null!;
		private IMediaBoxDbContext _dbContext = null!;
		public override void SetUp() {
			base.SetUp();
			var tagA = new Tag { TagName = "aa" };
			var tagB = new Tag { TagName = "bb" };
			var tagC = new Tag { TagName = "cc" };
			this._dbContextMockCreator = new DbContextMockCreator();
			this._dbContextMockCreator.SetData(tagA, tagB, tagC);
			this._testData = new() {
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, rate: 0, width: 30, height: 50, mediaFileTags: new[] { new MediaFileTag { Tag = tagA }, new MediaFileTag { Tag = tagB } }, latitude: 137.333, longitude: 31.121, altitude: null, position: new Position { Latitude = 137.333, Longitude = 31.121 }),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, rate: 1, width: 50, height: 30, mediaFileTags: new[] { new MediaFileTag { Tag = tagB } }, latitude: null, longitude: null, altitude: null),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, rate: 2, width: 10, height: 150, mediaFileTags: new[] { new MediaFileTag { Tag = tagC } }, latitude: 135.123, longitude: 34.121, position: new Position { Latitude = 135.123, Longitude = 34.121 }),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image4Png.FilePath, mediaFileId: 4, rate: 3, width: 1501, height: 1, latitude: null, longitude: null, altitude: null),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, rate: 4, width: 2, height: 7, latitude: null, longitude: null, altitude: null),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, rate: 5, width: 20, height: 70, mediaFileTags: new[] { new MediaFileTag { Tag = tagB } }, latitude: null, longitude: null, altitude: null)
			};
			this._dbContextMockCreator.SetData(this._testData.ToArray());
			this._dbContext = this._dbContextMockCreator.Mock.Object;
		}

		[Test]
		public void フィルター作成() {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.FilterObject.Should().Be(rfo);
		}

		[Test]
		public void フィルター名() {
			var rfo = new FilterObject {
				DisplayName = {
					Value = "FilterName1"
				}
			};
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.DisplayName.Value.Should().Be("FilterName1");
			sfc.DisplayName.Value = "FN2";
			rfo.DisplayName.Value.Should().Be("FN2");
			rfo.DisplayName.Value = "FN3";
			sfc.DisplayName.Value.Should().Be("FN3");
		}

		[Test]
		public void フィルター条件復元() {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using (var sfc = new FilteringCondition(rfo, filterItemFactory)) {
				sfc.AddTagFilter("bb", SearchTypeInclude.Exclude);
				sfc.AddFilePathFilter(".jpg", SearchTypeInclude.Include);
			}
			using (var sfc = new FilteringCondition(rfo, filterItemFactory)) {
				sfc.FilterItemObjects.Count.Should().Be(2);
				var tag = sfc.FilterItemObjects[0].As<TagFilterItemObject>();
				tag.TagName.Should().Be("bb");
				tag.SearchType.Should().Be(SearchTypeInclude.Exclude);
				var filepath = sfc.FilterItemObjects[1].As<FilePathFilterItemObject>();
				filepath.Text.Should().Be(".jpg");
				filepath.SearchType.Should().Be(SearchTypeInclude.Include);
				sfc.SetFilterConditions(this._dbContext.MediaFiles.Include(x => x.MediaFileTags).ThenInclude(x => x.Tag)).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 3, 5 });
			}
		}

		[Test]
		public void フィルターなし() {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
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
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddTagFilter(tag, searchType);
			sfc.SetFilterConditions(this._dbContext.MediaFiles.Include(x => x.MediaFileTags).ThenInclude(x => x.Tag)).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles.Include(x => x.MediaFileTags).ThenInclude(x => x.Tag)).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
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
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddFilePathFilter(filePath, searchType);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(1, SearchTypeComparison.GreaterThanOrEqual, 2, 3, 4, 5, 6)]
		[TestCase(1, SearchTypeComparison.GreaterThan, 3, 4, 5, 6)]
		[TestCase(2, SearchTypeComparison.GreaterThan, 4, 5, 6)]
		[TestCase(3, SearchTypeComparison.Equal, 4)]
		[TestCase(3, SearchTypeComparison.LessThan, 1, 2, 3)]
		[TestCase(3, SearchTypeComparison.LessThanOrEqual, 1, 2, 3, 4)]
		public void 評価フィルター追加削除(int rate, SearchTypeComparison searchType, params long[] result) {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddRateFilter(rate, searchType);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}


		[TestCase(150, 10, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 3, 4)]
		[TestCase(30, null, SearchTypeComparison.GreaterThan, 2, 4)]
		[TestCase(30, null, SearchTypeComparison.GreaterThanOrEqual, 1, 2, 4)]
		[TestCase(null, 30, SearchTypeComparison.LessThan, 4, 5)]
		[TestCase(null, 30, SearchTypeComparison.LessThanOrEqual, 2, 4, 5)]
		[TestCase(150, 10, SearchTypeComparison.Equal, 1, 2, 3)]
		public void 解像度フィルター追加削除(int? width, int? height, SearchTypeComparison searchType, params long[] result) {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddResolutionFilter(width, height, searchType);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(true, 6)]
		[TestCase(false, 1, 2, 3, 4, 5)]
		public void メディアタイプフィルター追加削除(bool isVideo, params long[] result) {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddMediaTypeFilter(isVideo);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(true, 1, 3)]
		[TestCase(false, 2, 4, 5, 6)]
		public void 座標フィルター追加削除(bool hasLocation, params long[] result) {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddLocationFilter(hasLocation);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[TestCase(true, 1, 2, 3, 4, 5, 6)]
		[TestCase(false, 7, 8)]
		public void 存在フィルター追加削除(bool exists, params long[] result) {
			var addingData = new[] {
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.NotExistsFileJpg.FilePath, mediaFileId: 7 ),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.NotExistsFileMov.FilePath, mediaFileId: 8 )
			};
			this._testData.AddRange(addingData);
			this._dbContextMockCreator.SetData(addingData);

			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddExistsFilter(exists);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(result);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6, 7, 8 });
		}

		[Test]
		public void 複数条件() {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddTagFilter("bb", SearchTypeInclude.Include);
			sfc.AddResolutionFilter(150, 10, SearchTypeComparison.GreaterThanOrEqual);
			sfc.SetFilterConditions(this._dbContext.MediaFiles.Include(x => x.MediaFileTags).ThenInclude(x => x.Tag)).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2 });
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			sfc.SetFilterConditions(this._dbContext.MediaFiles.Include(x => x.MediaFileTags).ThenInclude(x => x.Tag)).Select(x => x.MediaFileId).Should().BeEquivalentTo(new long[] { 1, 2, 3, 4, 5, 6 });
		}

		[Test]
		public void モデルに対するフィルター() {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			sfc.AddFilePathFilter(".jpg", SearchTypeInclude.Include);
			var addingData = new[] {
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.NotExistsFileJpg.FilePath, mediaFileId: 7),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.NotExistsFileMov.FilePath, mediaFileId: 8)
			};
			this._testData.AddRange(addingData);
			this._dbContextMockCreator.SetData(addingData);
			var files = this._testData.Select(x => x.ToModel());

			files.ForEach(x => x.UpdateFileInfo());
			sfc.SetFilterConditions(files).Select(x => x.FilePath).Should().BeEquivalentTo(this.TestFiles.Image1Jpg.FilePath, this.TestFiles.Image2Jpg.FilePath, this.TestFiles.Image3Jpg.FilePath, this.TestFiles.NoExifJpg.FilePath, this.TestFiles.NotExistsFileJpg.FilePath);

			sfc.AddExistsFilter(true);
			sfc.SetFilterConditions(files).Select(x => x.FilePath).Should().BeEquivalentTo(this.TestFiles.Image1Jpg.FilePath, this.TestFiles.Image2Jpg.FilePath, this.TestFiles.Image3Jpg.FilePath, this.TestFiles.NoExifJpg.FilePath);
		}

		[Test]
		public void フィルター変更通知() {
			var rfo = new FilterObject();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var sfc = new FilteringCondition(rfo, filterItemFactory);
			var count = 0;
			sfc.OnUpdateFilteringConditions.Subscribe(_ => {
				count++;
			});
			count.Should().Be(0);
			sfc.AddTagFilter("tag", SearchTypeInclude.Exclude);
			count.Should().Be(1);
			sfc.AddRateFilter(4, SearchTypeComparison.GreaterThan);
			count.Should().Be(2);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			count.Should().Be(3);
			sfc.RemoveFilter(sfc.FilterItemObjects[0]);
			count.Should().Be(4);
		}
	}
}
