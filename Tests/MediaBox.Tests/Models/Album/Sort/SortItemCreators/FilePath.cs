using System.ComponentModel;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort.SortItemCreators {
	internal class FilePath : ModelTestClassBase {

		private IMediaFileModel[] _testData;
		public override void SetUp() {
			base.SetUp();
			var mock1 = new Mock<IMediaFileModel>();
			mock1.Setup(x => x.FilePath).Returns(@"C:\test\image1.jpg");
			mock1.Setup(x => x.MediaFileId).Returns(1);
			mock1.Setup(x => x.Rate).Returns(2);
			var mock2 = new Mock<IMediaFileModel>();
			mock2.Setup(x => x.FilePath).Returns(@"C:\file\image2.png");
			mock2.Setup(x => x.MediaFileId).Returns(2);
			mock2.Setup(x => x.Rate).Returns(3);
			var mock3 = new Mock<IMediaFileModel>();
			mock3.Setup(x => x.FilePath).Returns(@"C:\test\image3.jpg");
			mock3.Setup(x => x.MediaFileId).Returns(3);
			mock3.Setup(x => x.Rate).Returns(2);
			var mock4 = new Mock<IMediaFileModel>();
			mock4.Setup(x => x.FilePath).Returns(@"D:\test\data\image4.jpg");
			mock4.Setup(x => x.MediaFileId).Returns(4);
			mock4.Setup(x => x.Rate).Returns(2);
			var mock5 = new Mock<IMediaFileModel>();
			mock5.Setup(x => x.FilePath).Returns(@"D:\test\file5.jpg");
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
			var sic = new SortItemCreator(SortItemKeys.FilePath);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 2, 1, 3, 4, 5 });
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 5, 4, 3, 1, 2 });
		}

		[Test]
		public void ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.FilePath, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 5, 4, 3, 1, 2 });
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 2, 1, 3, 4, 5 });
		}

		[Test]
		public void 追加ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.FilePath);
			var si2 = sic2.Create();
			si.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 1, 3, 4, 2, 5 });
			si.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 4, 3, 1, 5, 2 });
		}

		[Test]
		public void 追加ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.FilePath, ListSortDirection.Descending);
			var si2 = sic2.Create();
			si.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 4, 3, 1, 5, 2 });
			si.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 1, 3, 4, 2, 5 });
		}
	}
}
