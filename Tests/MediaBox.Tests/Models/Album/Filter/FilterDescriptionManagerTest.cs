
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album.Filter {
	internal class FilterDescriptionManagerTest : ModelTestClassBase {
		[Test]
		public void シングルトン() {
			var i1 = Get.Instance<FilterDescriptionManager>();
			var i2 = Get.Instance<FilterDescriptionManager>();
			i1.Is(i2);
		}

		[Test]
		public void フィルタリング条件追加() {
			var fdm = new FilterDescriptionManager();
			fdm.FilteringConditions.Count.Is(0);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Is(1);
			this.States.AlbumStates.FilteringConditions.Is(1);
			fdm.FilteringConditions.Select(x => x.FilterId).Is(1);
			fdm.AddCondition();
			fdm.FilteringConditions.Count.Is(2);
			this.States.AlbumStates.FilteringConditions.Is(1, 2);
			fdm.FilteringConditions.Select(x => x.FilterId).Is(1, 2);
		}

		[Test]
		public void フィルタリング条件削除() {
			var fdm = new FilterDescriptionManager();
			fdm.AddCondition();
			fdm.AddCondition();
			fdm.AddCondition();
			fdm.FilteringConditions.Select(x => x.FilterId).Is(1, 2, 3);
			fdm.RemoveCondition(fdm.FilteringConditions[0]);
			this.States.AlbumStates.FilteringConditions.Is(2, 3);
			fdm.FilteringConditions.Select(x => x.FilterId).Is(2, 3);
		}

		[Test]
		public void カレントフィルター変更() {
			var fdm = new FilterDescriptionManager();
			this.DataBase.MediaFiles.AddRange(new[] {
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image1Jpg.FilePath,mediaFileId:1,rate:0),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image2Jpg.FilePath,mediaFileId:2,rate:1),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image3Jpg.FilePath,mediaFileId:3,rate:2),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Image4Png.FilePath,mediaFileId:4,rate:3),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.NoExifJpg.FilePath,mediaFileId:5,rate:4),
				DatabaseUtility.GetMediaFileRecord(this.TestFiles.Video1Mov.FilePath,mediaFileId:6,rate:5),
			});
			this.DataBase.SaveChanges();
			fdm.AddCondition();
			fdm.AddCondition();
			fdm.AddCondition();

			var f1 = fdm.FilteringConditions[0];
			f1.AddRateFilter(2);
			var f2 = fdm.FilteringConditions[1];
			f2.AddRateFilter(3);
			var f3 = fdm.FilteringConditions[2];
			f3.AddRateFilter(4);

			// 更新通知回数
			var count = 0;
			fdm.OnFilteringConditionChanged.ObserveOn(Scheduler.Immediate).Subscribe(_ => count++);

			// フィルターなし
			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(1, 2, 3, 4, 5, 6);

			// f1に変更
			fdm.CurrentFilteringCondition.Value = f1;
			count.Is(1);
			this.States.AlbumStates.CurrentFilteringCondition.Value.Is(f1.FilterId);

			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(3, 4, 5, 6);
			f1.AddRateFilter(1);
			count.Is(2);

			// f2に変更
			fdm.CurrentFilteringCondition.Value = f2;
			count.Is(3);
			this.States.AlbumStates.CurrentFilteringCondition.Value.Is(f2.FilterId);

			fdm.SetFilterConditions(this.DataBase.MediaFiles).Select(x => x.MediaFileId).OrderBy(x => x).Is(4, 5, 6);
			f1.AddRateFilter(1);
			count.Is(3);

			f2.AddRateFilter(1);
			count.Is(4);
		}
	}
}