using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
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

		public void フィルター作成() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.RestorableFilterObject.Is(rfo);
		}

		[Test]
		public void フィルターなし() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void タグフィルター追加削除() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddTagFilter("bb", SearchTypeInclude.Include);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 6);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void ファイルパスフィルター追加削除() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddFilePathFilter("age3", SearchTypeInclude.Include);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(3);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void 解像度フィルター追加削除() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddResolutionFilter(150, 10);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void メディアタイプフィルター追加削除() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddMediaTypeFilter(true);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(6);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}

		[Test]
		public void 複数条件() {
			var rfo = new RestorableFilterObject();
			using var sfc = new FilteringCondition(rfo);
			sfc.AddTagFilter("bb", SearchTypeInclude.Include);
			sfc.AddResolutionFilter(150, 10);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.RemoveFilter(sfc.FilterItemCreators[0]);
			sfc.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
		}
	}
}
