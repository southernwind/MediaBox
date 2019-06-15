using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilteringConditionTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();

			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, rate: 0, width: 30, height: 50, tags: new[] { "aa", "bb" });
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, rate: 1, width: 50, height: 30, tags: new[] { "bb" });
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, rate: 2, width: 10, height: 150, tags: new[] { "cc" });
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, rate: 3, width: 1501, height: 1);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, rate: 4, width: 2, height: 7);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, rate: 5, width: 20, height: 70, tags: new[] { "bb" });
			this.DataBase.SaveChanges();
		}

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(2)]
		public void フィルター作成(int id) {
			var sfc = new FilteringCondition(id);
			sfc.FilterId.Is(id);
		}

		[Test]
		public void フィルターなし() {
			var sfc = new FilteringCondition(0);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void タグフィルター追加削除() {
			var sfc = new FilteringCondition(0);
			sfc.AddTagFilter("bb");
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 6);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void ファイルパスフィルター追加削除() {
			var sfc = new FilteringCondition(0);
			sfc.AddFilePathFilter("age3");
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(3);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void 解像度フィルター追加削除() {
			var sfc = new FilteringCondition(0);
			sfc.AddResolutionFilter(150, 10);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void メディアタイプフィルター追加削除() {
			var sfc = new FilteringCondition(0);
			sfc.AddMediaTypeFilter(true);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(6);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void 複数条件() {
			var sfc = new FilteringCondition(0);
			sfc.AddTagFilter("bb");
			sfc.AddResolutionFilter(150, 10);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void 保存復元() {
			using (var sfc = new FilteringCondition(55)) {
				sfc.AddTagFilter("bb");
				sfc.AddResolutionFilter(150, 10);
				sfc.Save();
			}
			// フィルターIDに基づいてフィルターを復元
			using (var sfc = new FilteringCondition(55)) {
				sfc.Load();
				sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2);
			}

			// フィルターIDが異なればフィルターは復元されない
			using (var sfc = new FilteringCondition(1)) {
				sfc.Load();
				sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
			}
		}
	}
}
