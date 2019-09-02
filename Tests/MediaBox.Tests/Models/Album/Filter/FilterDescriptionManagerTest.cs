
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Filter;
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

			// フィルターなし
			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);

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

			// 更新通知回数
			var count = 0;
			fdm.OnFilteringConditionChanged.ObserveOn(Scheduler.Immediate).Subscribe(_ => count++);

			// f1に変更
			fdm.CurrentFilteringCondition.Value = f1;
			count.Is(1);
			this.States.AlbumStates.CurrentFilteringCondition["main"].Is(f1.RestorableFilterObject);

			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(3, 4, 5, 6);
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Is(2);

			// f2に変更
			fdm.CurrentFilteringCondition.Value = f2;
			count.Is(3);
			this.States.AlbumStates.CurrentFilteringCondition["main"].Is(f2.RestorableFilterObject);

			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(4, 5, 6);
			f1.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Is(3);

			f2.AddRateFilter(1, SearchTypeComparison.GreaterThanOrEqual);
			count.Is(4);
		}
	}
}