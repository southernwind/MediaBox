using System;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort {
	internal class SortDescriptionManagerTest : ModelTestClassBase {

		[Test]
		public void 設定値読み込み() {
			using (var sdm = new SortDescriptionManager("main")) {
				this.States.AlbumStates.SortConditions.SetValue(Array.Empty<RestorableSortObject>());

				sdm.AddCondition();
				sdm.AddCondition();
				var s1 = sdm.SortConditions[0];
				var s2 = sdm.SortConditions[1];
				s1.DisplayName.Value = "name1";
				s1.AddSortItem(new SortItemCreator(SortItemKeys.FileName));
				s1.AddSortItem(new SortItemCreator(SortItemKeys.CreationTime, ListSortDirection.Descending));
				s2.DisplayName.Value = "name2";
				s2.AddSortItem(new SortItemCreator(SortItemKeys.LastAccessTime, ListSortDirection.Descending));
				s2.AddSortItem(new SortItemCreator(SortItemKeys.FileSize, ListSortDirection.Descending));

				sdm.CurrentSortCondition.Value = s1;
			}

			using var main = new SortDescriptionManager("main");
			using var sub = new SortDescriptionManager("sub");
			foreach (var sdm in new[] { main, sub }) {
				sdm.SortConditions.Count.Is(2);
				var s1 = sdm.SortConditions[0];
				var s2 = sdm.SortConditions[1];
				s1.DisplayName.Value.Is("name1");
				s1.SortItemCreators
					.Select(x => (x.SortItemKey, x.Direction))
					.Is(
						(SortItemKeys.FileName, ListSortDirection.Ascending),
						(SortItemKeys.CreationTime, ListSortDirection.Descending));
				s2.DisplayName.Value.Is("name2");
				s2.SortItemCreators
					.Select(x => (x.SortItemKey, x.Direction))
					.Is(
						(SortItemKeys.LastAccessTime, ListSortDirection.Descending),
						(SortItemKeys.FileSize, ListSortDirection.Descending));

				if (sdm == main) {
					sdm.CurrentSortCondition.Value.Is(s1);
				} else {
					sdm.CurrentSortCondition.Value.IsNull();
				}
			}
		}

		[Test]
		public void ソート設定追加削除() {
			using var sdm = new SortDescriptionManager("main");

			this.States.AlbumStates.SortConditions.SetValue(Array.Empty<RestorableSortObject>());
			sdm.SortConditions.IsEmpty();
			sdm.AddCondition();
			sdm.SortConditions.Count.Is(1);
			this.States.AlbumStates.SortConditions.Value.Is(sdm.SortConditions.First().RestorableSortObject);
			sdm.AddCondition();
			sdm.SortConditions[0].DisplayName.Value = "sort1";
			sdm.SortConditions[1].DisplayName.Value = "sort2";

			sdm.SortConditions.Select(x => x.DisplayName.Value).Is("sort1", "sort2");

			sdm.RemoveCondition(sdm.SortConditions[0]);
			sdm.SortConditions.Count.Is(1);
			sdm.SortConditions.Select(x => x.DisplayName.Value).Is("sort2");
		}

		[Test]
		public void ソート() {
			var m1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			m1.MediaFileId = 1;
			m1.Rate = 3;
			m1.FileSize = 500;
			var m2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			m2.MediaFileId = 2;
			m2.Rate = 4;
			m2.FileSize = 200;
			var m3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			m3.MediaFileId = 3;
			m3.Rate = 2;
			m3.FileSize = 300;
			var m4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			m4.MediaFileId = 4;
			m4.Rate = 0;
			m4.FileSize = 200;
			var m5 = this.MediaFactory.Create(this.TestFiles.NotExistsFileJpg.FilePath);
			m5.MediaFileId = 5;
			m5.Rate = 5;
			m5.FileSize = 400;
			var m6 = this.MediaFactory.Create(this.TestFiles.NotExistsFileMov.FilePath);
			m6.MediaFileId = 6;
			m6.Rate = 3;
			m6.FileSize = 200;

			var models = new[] { m1, m2, m3, m4, m5, m6 };

			using var sdm = new SortDescriptionManager("main");
			this.States.AlbumStates.SortConditions.SetValue(Array.Empty<RestorableSortObject>());

			sdm.SortConditions.Count.Is(0);
			sdm.CurrentSortCondition.Value.IsNull();

			sdm.SetSortConditions(models).Is(m1, m2, m3, m4, m5, m6);

			sdm.AddCondition();
			sdm.AddCondition();
			sdm.AddCondition();

			var sc1 = sdm.SortConditions[0];
			var sc2 = sdm.SortConditions[1];
			var sc3 = sdm.SortConditions[2];

			sc1.AddSortItem(new SortItemCreator(SortItemKeys.Rate, ListSortDirection.Descending));
			sc1.AddSortItem(new SortItemCreator(SortItemKeys.FilePath));

			sc2.AddSortItem(new SortItemCreator(SortItemKeys.FileName, ListSortDirection.Descending));

			sc3.AddSortItem(new SortItemCreator(SortItemKeys.FileSize));
			sc3.AddSortItem(new SortItemCreator(SortItemKeys.FilePath, ListSortDirection.Descending));

			sdm.CurrentSortCondition.Value = sc1;
			sdm.SetSortConditions(models).Is(m5, m2, m1, m6, m3, m4);

			sdm.CurrentSortCondition.Value = sc2;
			sdm.SetSortConditions(models).Is(m6, m5, m4, m3, m2, m1);

			sdm.CurrentSortCondition.Value = sc3;
			sdm.SetSortConditions(models).Is(m6, m4, m2, m3, m5, m1);

			sdm.Direction.Value = ListSortDirection.Descending;
			sdm.SetSortConditions(models).Is(m1, m5, m3, m2, m4, m6);

		}

		[Test]
		public void ソート条件変更通知() {
			using var sdm = new SortDescriptionManager("main");
			this.States.AlbumStates.SortConditions.SetValue(Array.Empty<RestorableSortObject>());

			sdm.AddCondition();
			sdm.AddCondition();
			sdm.AddCondition();

			var sc1 = sdm.SortConditions[0];
			var sc2 = sdm.SortConditions[1];
			var sc3 = sdm.SortConditions[2];

			sc1.AddSortItem(new SortItemCreator(SortItemKeys.Rate, ListSortDirection.Descending));
			sc1.AddSortItem(new SortItemCreator(SortItemKeys.FilePath));

			sc2.AddSortItem(new SortItemCreator(SortItemKeys.FileName, ListSortDirection.Descending));

			sc3.AddSortItem(new SortItemCreator(SortItemKeys.FileSize));
			sc3.AddSortItem(new SortItemCreator(SortItemKeys.FilePath, ListSortDirection.Descending));

			sdm.CurrentSortCondition.Value = sc1;

			var count = 0;
			sdm.OnSortConditionChanged.Subscribe(x => {
				count++;
			});


			sc1.AddSortItem(new SortItemCreator(SortItemKeys.CreationTime));
			count.Is(1);

			sc2.AddSortItem(new SortItemCreator(SortItemKeys.FileName));
			count.Is(1);

			sdm.CurrentSortCondition.Value = sc2;
			count.Is(2);

			sc2.AddSortItem(new SortItemCreator(SortItemKeys.LastAccessTime));
			count.Is(3);

			sc1.AddSortItem(new SortItemCreator(SortItemKeys.ModifiedTime));
			count.Is(3);

			sc2.RemoveSortItem(sc2.SortItemCreators[0]);
			count.Is(4);

			sdm.Direction.Value = ListSortDirection.Descending;
			count.Is(5);

			sdm.Direction.Value = ListSortDirection.Ascending;
			count.Is(6);
		}
	}
}
