
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilterDescriptionManagerTest : ModelTestClassBase {
		[Test]
		public void フィルタリング条件追加() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var fdm = new FilterDescriptionManager(statesMock.Object, settingsMock.Object);
			fdm.Name.Value = "main";
			fdm.FilteringConditions.Count.Should().Be(0);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Should().Be(1);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Should().Be(2);
			var f1 = fdm.FilteringConditions[0];
			var f2 = fdm.FilteringConditions[1];
			statesMock.Object.AlbumStates.FilteringConditions.Should().Equal(new[] { f1.RestorableFilterObject, f2.RestorableFilterObject });
		}

		[Test]
		public void フィルタリング条件削除() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var fdm = new FilterDescriptionManager(statesMock.Object, settingsMock.Object);
			fdm.Name.Value = "main";
			fdm.AddCondition();
			fdm.AddCondition();
			fdm.AddCondition();
			var f1 = fdm.FilteringConditions[0];
			var f2 = fdm.FilteringConditions[1];
			var f3 = fdm.FilteringConditions[2];
			fdm.RemoveCondition(f1);
			fdm.FilteringConditions.Should().Equal(new[] { f2, f3 });
			statesMock.Object.AlbumStates.FilteringConditions.Should().Equal(new[] { f2.RestorableFilterObject, f3.RestorableFilterObject });
		}

		[Test]
		public void カレントフィルター変更() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using var fdm = new FilterDescriptionManager(statesMock.Object, settingsMock.Object);
			fdm.Name.Value = "main";
			var testTableData = new[] {
				new MediaFile { FilePath = this.TestFiles.Image1Jpg.FilePath, MediaFileId = 1, Rate = 0 },
				new MediaFile { FilePath = this.TestFiles.Image2Jpg.FilePath, MediaFileId = 2, Rate = 1 },
				new MediaFile { FilePath = this.TestFiles.Image3Jpg.FilePath, MediaFileId = 3, Rate = 2 },
				new MediaFile { FilePath = this.TestFiles.Image4Png.FilePath, MediaFileId = 4, Rate = 3 },
				new MediaFile { FilePath = this.TestFiles.NoExifJpg.FilePath, MediaFileId = 5, Rate = 4 },
				new MediaFile { FilePath = this.TestFiles.Video1Mov.FilePath, MediaFileId = 6, Rate = 5 }
			};
			var testModelData = testTableData.Select(r => r.ToModel());

			// フィルターなし
			fdm.SetFilterConditions(testTableData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long[] { 1, 2, 3, 4, 5, 6 });
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long?[] { 1, 2, 3, 4, 5, 6 });

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
			fdm.SetFilterConditions(testTableData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long[] { 5, 6 });
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long?[] { 5, 6 });

			// 更新通知回数
			var count = 0;
			fdm.OnFilteringConditionChanged.ObserveOn(Scheduler.Immediate).Subscribe(_ => count++);

			// f1に変更
			fdm.CurrentFilteringCondition.Value = f1;
			count.Should().Be(1);
			statesMock.Object.AlbumStates.CurrentFilteringCondition["main"].Should().Be(f1.RestorableFilterObject);

			fdm.SetFilterConditions(testTableData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long[] { 3, 4, 5, 6 });
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long?[] { 3, 4, 5, 6 });
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Should().Be(2);

			// f2に変更
			fdm.CurrentFilteringCondition.Value = f2;
			count.Should().Be(3);
			statesMock.Object.AlbumStates.CurrentFilteringCondition["main"].Should().Be(f2.RestorableFilterObject);

			fdm.SetFilterConditions(testTableData.ToLiteDbCollection().Query()).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long[] { 4, 5, 6 });
			fdm.SetFilterConditions(testModelData).Select(x => x.MediaFileId).OrderBy(x => x).Should().Equal(new long?[] { 4, 5, 6 });
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Should().Be(3);

			f2.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Should().Be(4);
		}

		[Test]
		public void 設定値の保存復元() {
			var statesMock = ModelMockCreator.CreateStatesMock();
			var settingsMock = ModelMockCreator.CreateSettingsMock();
			using (var fdm = new FilterDescriptionManager(statesMock.Object, settingsMock.Object)) {
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

			using var main = new FilterDescriptionManager(statesMock.Object, settingsMock.Object);
			using var sub = new FilterDescriptionManager(statesMock.Object, settingsMock.Object);
			main.Name.Value = "main";
			sub.Name.Value = "sub";
			foreach (var fdm in new[] { main, sub }) {
				var f1 = fdm.FilteringConditions[0];
				var f2 = fdm.FilteringConditions[1];
				var f3 = fdm.FilteringConditions[2];
				f1.DisplayName.Value.Should().Be("filter1");
				var f1Rate = f1.FilterItemCreators[0].As<RateFilterItemCreator>();
				f1Rate.Rate.Should().Be(3);
				f1Rate.SearchType.Should().Be(SearchTypeComparison.Equal);
				f2.DisplayName.Value.Should().Be("filter2");
				var f2Media = f2.FilterItemCreators[0].As<MediaTypeFilterItemCreator>();
				var f2Exists = f2.FilterItemCreators[1].As<ExistsFilterItemCreator>();
				f2Media.IsVideo.Should().BeTrue();
				f2Exists.Exists.Should().BeTrue();
				f3.DisplayName.Value.Should().Be("filter3");
				var f3Filepath = f3.FilterItemCreators[0].As<FilePathFilterItemCreator>();
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