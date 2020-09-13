using System.ComponentModel;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.TestUtilities.MockCreator;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort.SortItemCreators {
	internal class FileName : ModelTestClassBase {

		private IMediaFileModel[] _testData = null!;
		public override void SetUp() {
			base.SetUp();
			var mock1 = ModelMockCreator.CreateMediaFileModelMock();
			mock1.Setup(x => x.FileName).Returns("image1.jpg");
			mock1.Setup(x => x.MediaFileId).Returns(1);
			mock1.Setup(x => x.Rate).Returns(2);
			var mock2 = ModelMockCreator.CreateMediaFileModelMock();
			mock2.Setup(x => x.FileName).Returns("image2.jpg");
			mock2.Setup(x => x.MediaFileId).Returns(2);
			mock2.Setup(x => x.Rate).Returns(3);
			var mock3 = ModelMockCreator.CreateMediaFileModelMock();
			mock3.Setup(x => x.FileName).Returns("image3.jpg");
			mock3.Setup(x => x.MediaFileId).Returns(3);
			mock3.Setup(x => x.Rate).Returns(2);
			var mock4 = ModelMockCreator.CreateMediaFileModelMock();
			mock4.Setup(x => x.FileName).Returns("image4.jpg");
			mock4.Setup(x => x.MediaFileId).Returns(4);
			mock4.Setup(x => x.Rate).Returns(2);
			var mock5 = ModelMockCreator.CreateMediaFileModelMock();
			mock5.Setup(x => x.FileName).Returns("file5.jpg");
			mock5.Setup(x => x.MediaFileId).Returns(5);
			mock5.Setup(x => x.Rate).Returns(3);

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
			var sic = new SortItemCreator(SortItemKeys.FileName);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().Equal(5, 1, 2, 3, 4);
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().Equal(4, 3, 2, 1, 5);
		}

		[Test]
		public void ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.FileName, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().Equal(4, 3, 2, 1, 5);
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().Equal(5, 1, 2, 3, 4);
		}

		[Test]
		public void 追加ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.FileName);
			var si2 = sic2.Create();
			si2.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().Equal(1, 3, 4, 5, 2);
			si2.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().Equal(4, 3, 1, 2, 5);
		}

		[Test]
		public void 追加ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.FileName, ListSortDirection.Descending);
			var si2 = sic2.Create();
			si2.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().Equal(4, 3, 1, 2, 5);
			si2.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().Equal(1, 3, 4, 5, 2);
		}
	}
}
