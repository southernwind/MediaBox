using System.ComponentModel;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort.SortItemCreators {
	internal class Location : ModelTestClassBase {

		private IMediaFileModel[] _testData;
		public override void SetUp() {
			base.SetUp();
			var mock1 = ModelMockCreator.CreateMediaFileModelMock();
			mock1.Setup(x => x.MediaFileId).Returns(1);
			mock1.Setup(x => x.Rate).Returns(2);
			mock1.Setup(x => x.Location).Returns(new GpsLocation(10, 10));
			var mock2 = ModelMockCreator.CreateMediaFileModelMock();
			mock2.Setup(x => x.MediaFileId).Returns(2);
			mock2.Setup(x => x.Rate).Returns(3);
			mock2.Setup(x => x.Location).Returns(new GpsLocation(20, 50, 31));
			var mock3 = ModelMockCreator.CreateMediaFileModelMock();
			mock3.Setup(x => x.MediaFileId).Returns(3);
			mock3.Setup(x => x.Rate).Returns(2);
			mock3.Setup(x => x.Location).Returns(new GpsLocation(20, 50, 30));
			var mock4 = ModelMockCreator.CreateMediaFileModelMock();
			mock4.Setup(x => x.MediaFileId).Returns(4);
			mock4.Setup(x => x.Rate).Returns(2);
			mock4.Setup(x => x.Location).Returns(new GpsLocation(1, 180));
			var mock5 = ModelMockCreator.CreateMediaFileModelMock();
			mock5.Setup(x => x.MediaFileId).Returns(5);
			mock5.Setup(x => x.Rate).Returns(3);
			mock5.Setup(x => x.Location).Returns(new GpsLocation(10, -45));

			this._testData = new[] {
				mock1.Object,
				mock2.Object,
				mock3.Object,
				mock4.Object,
				mock5.Object
			};
		}

		[Test]
		public void ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.Location);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().BeEquivalentTo(4, 5, 1, 3, 2);
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().BeEquivalentTo(2, 3, 1, 5, 4);
		}

		[Test]
		public void ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.Location, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().BeEquivalentTo(2, 3, 1, 5, 4);
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().BeEquivalentTo(4, 5, 1, 3, 2);
		}

		[Test]
		public void 追加ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.Location);
			var si2 = sic2.Create();
			si.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().BeEquivalentTo(4, 1, 3, 5, 2);
			si.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().BeEquivalentTo(3, 1, 4, 2, 5);
		}

		[Test]
		public void 追加ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.Location, ListSortDirection.Descending);
			var si2 = sic2.Create();
			si.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().BeEquivalentTo(3, 1, 4, 2, 5);
			si.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().BeEquivalentTo(4, 1, 3, 5, 2);
		}
	}
}
