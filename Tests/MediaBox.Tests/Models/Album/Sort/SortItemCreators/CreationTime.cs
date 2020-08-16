using System;
using System.ComponentModel;
using System.Linq;

using FluentAssertions;

using Moq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort.SortItemCreators {
	internal class CreationTime : ModelTestClassBase {
		private IMediaFileModel[] _testData;
		public override void SetUp() {
			base.SetUp();
			var mock1 = new Mock<IMediaFileModel>();
			mock1.Setup(x => x.FilePath).Returns(this.TestFiles.Image1Jpg.FilePath);
			mock1.Setup(x => x.MediaFileId).Returns(1);
			mock1.Setup(x => x.Rate).Returns(2);
			mock1.Setup(x => x.CreationTime).Returns(new DateTime(2019, 10, 1, 0, 0, 0));
			var mock2 = new Mock<IMediaFileModel>();
			mock2.Setup(x => x.FilePath).Returns(this.TestFiles.Image2Jpg.FilePath);
			mock2.Setup(x => x.MediaFileId).Returns(2);
			mock2.Setup(x => x.Rate).Returns(3);
			mock2.Setup(x => x.CreationTime).Returns(new DateTime(2022, 8, 1, 0, 0, 0));
			var mock3 = new Mock<IMediaFileModel>();
			mock3.Setup(x => x.FilePath).Returns(this.TestFiles.Image3Jpg.FilePath);
			mock3.Setup(x => x.MediaFileId).Returns(3);
			mock3.Setup(x => x.Rate).Returns(2);
			mock3.Setup(x => x.CreationTime).Returns(new DateTime(2014, 6, 30, 11, 5, 30));
			var mock4 = new Mock<IMediaFileModel>();
			mock4.Setup(x => x.FilePath).Returns(this.TestFiles.Image4Png.FilePath);
			mock4.Setup(x => x.MediaFileId).Returns(4);
			mock4.Setup(x => x.Rate).Returns(2);
			mock4.Setup(x => x.CreationTime).Returns(new DateTime(2014, 6, 30, 11, 5, 31));
			var mock5 = new Mock<IMediaFileModel>();
			mock5.Setup(x => x.FilePath).Returns(this.TestFiles.Video1Mov.FilePath);
			mock5.Setup(x => x.MediaFileId).Returns(5);
			mock5.Setup(x => x.Rate).Returns(3);
			mock5.Setup(x => x.CreationTime).Returns(new DateTime(2014, 6, 4, 0, 0, 0));

			this._testData = new[] {
				mock1.Object,
				mock2.Object,
				mock3.Object,
				mock4.Object,
				mock5.Object
			};
		}

		public IOrderedEnumerable<IMediaFileModel> CreateSortedModels() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			return si.ApplySort(this._testData, false);
		}

		[Test]
		public void ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.CreationTime);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 5, 3, 4, 1, 2 });
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 2, 1, 4, 3, 5 });
		}

		[Test]
		public void ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.CreationTime, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplySort(this._testData, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 2, 1, 4, 3, 5 });
			si.ApplySort(this._testData, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 5, 3, 4, 1, 2 });
		}

		[Test]
		public void 追加ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.CreationTime);
			var si2 = sic2.Create();
			si2.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 3, 4, 1, 5, 2 });
			si2.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 1, 4, 3, 2, 5 });
		}

		[Test]
		public void 追加ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			var ordered = si.ApplySort(this._testData, false);
			var sic2 = new SortItemCreator(SortItemKeys.CreationTime, ListSortDirection.Descending);
			var si2 = sic2.Create();
			si2.ApplyThenBySort(ordered, false).Select(x => x.MediaFileId).Should().Equal(new long?[] { 1, 4, 3, 2, 5 });
			si2.ApplyThenBySort(ordered, true).Select(x => x.MediaFileId).Should().Equal(new long?[] { 3, 4, 1, 5, 2 });
		}
	}
}
