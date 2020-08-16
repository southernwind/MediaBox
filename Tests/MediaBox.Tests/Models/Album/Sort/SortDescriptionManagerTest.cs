using System;
using System.ComponentModel;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort {
	internal class SortDescriptionManagerTest : ModelTestClassBase {

		[Test]
		public void 設定値読み込み() {
			var statesMock = new Mock<IStates>();
			statesMock.SetupAllProperties();
			using (var sdm = new SortDescriptionManager(statesMock.Object)) {
				sdm.Name.Value = "main";

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

			using var main = new SortDescriptionManager(statesMock.Object);
			main.Name.Value = "main";
			using var sub = new SortDescriptionManager(statesMock.Object);
			sub.Name.Value = "sub";
			foreach (var sdm in new[] { main, sub }) {
				sdm.SortConditions.Count.Should().Be(2);
				var s1 = sdm.SortConditions[0];
				var s2 = sdm.SortConditions[1];
				s1.DisplayName.Value.Should().Be("name1");
				s1.SortItemCreators
					.Select(x => (x.SortItemKey, x.Direction))
					.Should().Equal(new[] {
						(SortItemKeys.FileName, ListSortDirection.Ascending),
						(SortItemKeys.CreationTime, ListSortDirection.Descending)
					});
				s2.DisplayName.Value.Should().Be("name2");
				s2.SortItemCreators
					.Select(x => (x.SortItemKey, x.Direction))
					.Should().Equal(new[] {
						(SortItemKeys.LastAccessTime, ListSortDirection.Descending),
						(SortItemKeys.FileSize, ListSortDirection.Descending)
					});

				if (sdm == main) {
					sdm.CurrentSortCondition.Value.Should().Be(s1);
				} else {
					sdm.CurrentSortCondition.Value.Should().BeNull();
				}
			}
		}

		[Test]
		public void ソート設定追加削除() {
			var statesMock = new Mock<IStates>();
			statesMock.SetupAllProperties();
			using var sdm = new SortDescriptionManager(statesMock.Object);
			sdm.Name.Value = "main";

			sdm.SortConditions.IsEmpty();
			sdm.AddCondition();
			sdm.SortConditions.Count.Should().Be(1);
			statesMock.Object.AlbumStates.SortConditions.Value.Should().Equal(new[] { sdm.SortConditions.First().RestorableSortObject });
			sdm.AddCondition();
			sdm.SortConditions[0].DisplayName.Value = "sort1";
			sdm.SortConditions[1].DisplayName.Value = "sort2";

			sdm.SortConditions.Select(x => x.DisplayName.Value).Should().Equal(new[] { "sort1", "sort2" });

			sdm.RemoveCondition(sdm.SortConditions[0]);
			sdm.SortConditions.Count.Should().Be(1);
			sdm.SortConditions.Select(x => x.DisplayName.Value).Should().Equal(new[] { "sort2" });
		}

		[Test]
		public void ソート() {

			var mock1 = new Mock<IMediaFileModel>();
			mock1.Setup(x => x.FilePath).Returns(this.TestFiles.Image1Jpg.FilePath);
			mock1.Setup(x => x.MediaFileId).Returns(1);
			mock1.Setup(x => x.Rate).Returns(3);
			mock1.Setup(x => x.FileSize).Returns(500);
			var m1 = mock1.Object;
			var mock2 = new Mock<IMediaFileModel>();
			mock2.Setup(x => x.FilePath).Returns(this.TestFiles.Image2Jpg.FilePath);
			mock2.Setup(x => x.MediaFileId).Returns(2);
			mock2.Setup(x => x.Rate).Returns(4);
			mock2.Setup(x => x.FileSize).Returns(200);
			var m2 = mock2.Object;
			var mock3 = new Mock<IMediaFileModel>();
			mock3.Setup(x => x.FilePath).Returns(this.TestFiles.Image3Jpg.FilePath);
			mock3.Setup(x => x.MediaFileId).Returns(3);
			mock3.Setup(x => x.Rate).Returns(2);
			mock3.Setup(x => x.FileSize).Returns(300);
			var m3 = mock3.Object;
			var mock4 = new Mock<IMediaFileModel>();
			mock4.Setup(x => x.FilePath).Returns(this.TestFiles.Image4Png.FilePath);
			mock4.Setup(x => x.MediaFileId).Returns(4);
			mock4.Setup(x => x.Rate).Returns(0);
			mock4.Setup(x => x.FileSize).Returns(200);
			var m4 = mock4.Object;
			var mock5 = new Mock<IMediaFileModel>();
			mock5.Setup(x => x.FilePath).Returns(this.TestFiles.NotExistsFileJpg.FilePath);
			mock5.Setup(x => x.MediaFileId).Returns(5);
			mock5.Setup(x => x.Rate).Returns(5);
			mock5.Setup(x => x.FileSize).Returns(400);
			var m5 = mock5.Object;
			var mock6 = new Mock<IMediaFileModel>();
			mock6.Setup(x => x.FilePath).Returns(this.TestFiles.NotExistsFileMov.FilePath);
			mock6.Setup(x => x.MediaFileId).Returns(6);
			mock6.Setup(x => x.Rate).Returns(3);
			mock6.Setup(x => x.FileSize).Returns(200);
			var m6 = mock6.Object;

			var models = new[] { m1, m2, m3, m4, m5, m6 };

			var statesMock = new Mock<IStates>();
			statesMock.SetupAllProperties();
			using var sdm = new SortDescriptionManager(statesMock.Object);
			sdm.Name.Value = "main";

			sdm.SortConditions.Count.Should().Be(0);
			sdm.CurrentSortCondition.Value.Should().BeNull();

			sdm.SetSortConditions(models).Should().Equal(new[] { m1, m2, m3, m4, m5, m6 });

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
			sdm.SetSortConditions(models).Should().Equal(new[] { m5, m2, m1, m6, m3, m4 });

			sdm.CurrentSortCondition.Value = sc2;
			sdm.SetSortConditions(models).Should().Equal(new[] { m6, m5, m4, m3, m2, m1 });

			sdm.CurrentSortCondition.Value = sc3;
			sdm.SetSortConditions(models).Should().Equal(new[] { m6, m4, m2, m3, m5, m1 });

			sdm.Direction.Value = ListSortDirection.Descending;
			sdm.SetSortConditions(models).Should().Equal(new[] { m1, m5, m3, m2, m4, m6 });

		}

		[Test]
		public void ソート条件変更通知() {
			var statesMock = new Mock<IStates>();
			statesMock.SetupAllProperties();
			using var sdm = new SortDescriptionManager(statesMock.Object);
			sdm.Name.Value = "main";

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
			count.Should().Be(1);

			sc2.AddSortItem(new SortItemCreator(SortItemKeys.FileName));
			count.Should().Be(1);

			sdm.CurrentSortCondition.Value = sc2;
			count.Should().Be(2);

			sc2.AddSortItem(new SortItemCreator(SortItemKeys.LastAccessTime));
			count.Should().Be(3);

			sc1.AddSortItem(new SortItemCreator(SortItemKeys.ModifiedTime));
			count.Should().Be(3);

			sc2.RemoveSortItem(sc2.SortItemCreators[0]);
			count.Should().Be(4);

			sdm.Direction.Value = ListSortDirection.Descending;
			count.Should().Be(5);

			sdm.Direction.Value = ListSortDirection.Ascending;
			count.Should().Be(6);
		}
	}
}
