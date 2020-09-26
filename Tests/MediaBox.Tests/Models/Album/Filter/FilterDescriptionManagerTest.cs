
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilterDescriptionManagerTest : ModelTestClassBase {
		[Test]
		public void フィルタリング条件追加() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var fdm = new FilterDescriptionManager(statesMock.Object, filterItemFactory);
			fdm.Name.Value = "main";
			fdm.FilteringConditions.Count.Should().Be(0);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Should().Be(1);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Should().Be(2);
			var f1 = fdm.FilteringConditions[0];
			var f2 = fdm.FilteringConditions[1];
			statesMock.Object.AlbumStates.FilteringConditions.Should().Equal(f1.FilterObject, f2.FilterObject);
		}

		[Test]
		public void フィルタリング条件削除() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var fdm = new FilterDescriptionManager(statesMock.Object, filterItemFactory);
			fdm.Name.Value = "main";
			fdm.AddCondition();
			fdm.AddCondition();
			fdm.AddCondition();
			var f1 = fdm.FilteringConditions[0];
			var f2 = fdm.FilteringConditions[1];
			var f3 = fdm.FilteringConditions[2];
			fdm.RemoveCondition(f1);
			fdm.FilteringConditions.Should().Equal(f2, f3);
			statesMock.Object.AlbumStates.FilteringConditions.Should().Equal(f2.FilterObject, f3.FilterObject);
		}

		[Test]
		public void カレントフィルター変更() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using var fdm = new FilterDescriptionManager(statesMock.Object, filterItemFactory);
			fdm.Name.Value = "main";
			var testTableData = new[] {
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image1Jpg.FilePath, mediaFileId: 1, rate: 0),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image2Jpg.FilePath, mediaFileId: 2, rate: 1),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image3Jpg.FilePath, mediaFileId: 3, rate: 2),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Image4Png.FilePath, mediaFileId: 4, rate: 3),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.NoExifJpg.FilePath, mediaFileId: 5, rate: 4),
				DatabaseUtility.CreateMediaFileRecord(filePath: this.TestFiles.Video1Mov.FilePath, mediaFileId: 6, rate: 5)
			};
			var testModelData = testTableData.Select(r => r.ToModel());

			// フィルターなし
			fdm.SetFilterConditions(testTableData.AsQueryable()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(1, 2, 3, 4, 5, 6);
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(1, 2, 3, 4, 5, 6);

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
			fdm.SetFilterConditions(testTableData.AsQueryable()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(5, 6);
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(5, 6);

			// 更新通知回数
			var count = 0;
			fdm.OnFilteringConditionChanged.ObserveOn(Scheduler.Immediate).Subscribe(_ => count++);

			// f1に変更
			fdm.CurrentFilteringCondition.Value = f1;
			count.Should().Be(1);
			statesMock.Object.AlbumStates.CurrentFilteringCondition["main"].Should().Be(f1.FilterObject);

			fdm.SetFilterConditions(testTableData.AsQueryable()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(3, 4, 5, 6);
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(3, 4, 5, 6);
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Should().Be(2);

			// f2に変更
			fdm.CurrentFilteringCondition.Value = f2;
			count.Should().Be(3);
			statesMock.Object.AlbumStates.CurrentFilteringCondition["main"].Should().Be(f2.FilterObject);

			fdm.SetFilterConditions(testTableData.AsQueryable()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(4, 5, 6);
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(4, 5, 6);
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Should().Be(3);

			f2.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Should().Be(4);
		}

		[Test]
		public void 設定値の保存復元() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var filterItemFactory = ModelMockCreator.CreateFilterItemFactory();
			using (var fdm = new FilterDescriptionManager(statesMock.Object, filterItemFactory)) {
				fdm.Name.Value = "main";
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

			using var main = new FilterDescriptionManager(statesMock.Object, filterItemFactory);
			using var sub = new FilterDescriptionManager(statesMock.Object, filterItemFactory);
			main.Name.Value = "main";
			sub.Name.Value = "sub";
			foreach (var fdm in new[] { main, sub }) {
				var f1 = fdm.FilteringConditions[0];
				var f2 = fdm.FilteringConditions[1];
				var f3 = fdm.FilteringConditions[2];
				f1.DisplayName.Value.Should().Be("filter1");
				var f1Rate = f1.FilterItemObjects[0].As<RateFilterItemObject>();
				f1Rate.Rate.Should().Be(3);
				f1Rate.SearchType.Should().Be(SearchTypeComparison.Equal);
				f2.DisplayName.Value.Should().Be("filter2");
				var f2Media = f2.FilterItemObjects[0].As<MediaTypeFilterItemObject>();
				var f2Exists = f2.FilterItemObjects[1].As<ExistsFilterItemObject>();
				f2Media.IsVideo.Should().BeTrue();
				f2Exists.Exists.Should().BeTrue();
				f3.DisplayName.Value.Should().Be("filter3");
				var f3Filepath = f3.FilterItemObjects[0].As<FilePathFilterItemObject>();
				f3Filepath.Text.Should().Be("image");
				f3Filepath.SearchType.Should().Be(SearchTypeInclude.Exclude);

				if (fdm == main) {
					fdm.CurrentFilteringCondition.Value.Should().Be(f2);
				} else {
					fdm.CurrentFilteringCondition.Value.Should().BeNull();
				}
			}
		}
	}
}