
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilterDescriptionManagerTest : ModelTestClassBase {
		[Test]
		public void フィルタリング条件追加() {
			using var fdm = new FilterDescriptionManager("main");
			fdm.FilteringConditions.Count.Is(0);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Is(1);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Is(2);
			var f1 = fdm.FilteringConditions[0];
			var f2 = fdm.FilteringConditions[1];
			this.States.AlbumStates.FilteringConditions.Is(f1.RestorableFilterObject, f2.RestorableFilterObject);
		}

		[Test]
		public void フィルタリング条件削除() {
			using var fdm = new FilterDescriptionManager("main");
			fdm.AddCondition();
			fdm.AddCondition();
			fdm.AddCondition();
			var f1 = fdm.FilteringConditions[0];
			var f2 = fdm.FilteringConditions[1];
			var f3 = fdm.FilteringConditions[2];
			fdm.RemoveCondition(f1);
			fdm.FilteringConditions.Is(f2, f3);
			this.States.AlbumStates.FilteringConditions.Is(f2.RestorableFilterObject, f3.RestorableFilterObject);
		}

		[Test]
		public void カレントフィルター変更() {
			using var fdm = new FilterDescriptionManager("main");
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, rate: 0);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, rate: 1);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, rate: 2);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Image4Png.FilePath, mediaFileId: 4, rate: 3);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, rate: 4);
			DatabaseUtility.RegisterMediaFileRecord(this.DataBase, this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, rate: 5);

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

			var models = new[] { m1, m2, m3, m4, m5, m6 };

			// フィルターなし
			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);
			fdm.SetFilterConditions(models).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);

			fdm.AddCondition();
			fdm.AddCondition();
			fdm.AddCondition();

			var f1 = fdm.FilteringConditions[0];
			f1.AddRateFilter(2, SearchTypeComparison.GreaterThanOrEqual);
			var f2 = fdm.FilteringConditions[1];
			f2.AddRateFilter(3, SearchTypeComparison.GreaterThanOrEqual);
			var f3 = fdm.FilteringConditions[2];
			f3.AddRateFilter(4, SearchTypeComparison.GreaterThanOrEqual);

			// フィルター追加後は最後に追加したf3になっている
			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(5, 6);
			fdm.SetFilterConditions(models).Select(x => x.MediaFileId).OrderBy(x => x).Is(5, 6);

			// 更新通知回数
			var count = 0;
			fdm.OnFilteringConditionChanged.ObserveOn(Scheduler.Immediate).Subscribe(_ => count++);

			// f1に変更
			fdm.CurrentFilteringCondition.Value = f1;
			count.Is(1);
			this.States.AlbumStates.CurrentFilteringCondition["main"].Is(f1.RestorableFilterObject);

			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(3, 4, 5, 6);
			fdm.SetFilterConditions(models).Select(x => x.MediaFileId).OrderBy(x => x).Is(3, 4, 5, 6);
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Is(2);

			// f2に変更
			fdm.CurrentFilteringCondition.Value = f2;
			count.Is(3);
			this.States.AlbumStates.CurrentFilteringCondition["main"].Is(f2.RestorableFilterObject);

			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(4, 5, 6);
			fdm.SetFilterConditions(models).Select(x => x.MediaFileId).OrderBy(x => x).Is(4, 5, 6);
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Is(3);

			f2.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Is(4);
		}

		[Test]
		public void 設定値の保存復元() {
			using (var fdm = new FilterDescriptionManager("main")) {
				fdm.AddCondition();
				fdm.AddCondition();
				fdm.AddCondition();

				var f1 = fdm.FilteringConditions[0];
				var f2 = fdm.FilteringConditions[1];
				var f3 = fdm.FilteringConditions[2];

				f1.DisplayName.Value = "filter1";
				f1.AddRateFilter(3, SearchTypeComparison.Equal);
				f2.DisplayName.Value = "filter2";
				f2.AddMediaTypeFilter(true);
				f2.AddExistsFilter(true);
				f3.DisplayName.Value = "filter3";
				f3.AddFilePathFilter("image", SearchTypeInclude.Exclude);

				fdm.CurrentFilteringCondition.Value = f2;
			}

			using var main = new FilterDescriptionManager("main");
			using var sub = new FilterDescriptionManager("sub");
			foreach (var fdm in new[] { main, sub }) {
				var f1 = fdm.FilteringConditions[0];
				var f2 = fdm.FilteringConditions[1];
				var f3 = fdm.FilteringConditions[2];
				f1.DisplayName.Value.Is("filter1");
				var f1Rate = f1.FilterItemCreators[0].IsInstanceOf<RateFilterItemCreator>();
				f1Rate.Rate.Is(3);
				f1Rate.SearchType.Is(SearchTypeComparison.Equal);
				f2.DisplayName.Value.Is("filter2");
				var f2Media = f2.FilterItemCreators[0].IsInstanceOf<MediaTypeFilterItemCreator>();
				var f2Exists = f2.FilterItemCreators[1].IsInstanceOf<ExistsFilterItemCreator>();
				f2Media.IsVideo.IsTrue();
				f2Exists.Exists.IsTrue();
				f3.DisplayName.Value.Is("filter3");
				var f3Filepath = f3.FilterItemCreators[0].IsInstanceOf<FilePathFilterItemCreator>();
				f3Filepath.Text.Is("image");
				f3Filepath.SearchType.Is(SearchTypeInclude.Exclude);

				if (fdm == main) {
					fdm.CurrentFilteringCondition.Value.Is(f2);
				} else {
					fdm.CurrentFilteringCondition.Value.IsNull();
				}
			}
		}
	}
}