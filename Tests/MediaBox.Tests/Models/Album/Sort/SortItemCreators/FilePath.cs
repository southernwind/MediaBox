using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort.SortItemCreators {
	internal class FilePath : ModelTestClassBase {

		public IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(@"C:\test\image1.jpg");
			m1.MediaFileId = 1;
			m1.Rate = 2;
			var m2 = this.MediaFactory.Create(@"C:\file\image2.png");
			m2.MediaFileId = 2;
			m2.Rate = 3;
			var m3 = this.MediaFactory.Create(@"C:\test\image3.jpg");
			m3.MediaFileId = 3;
			m3.Rate = 2;
			var m4 = this.MediaFactory.Create(@"D:\test\data\image4.jpg");
			m4.MediaFileId = 4;
			m4.Rate = 2;
			var m5 = this.MediaFactory.Create(@"D:\test\file5.jpg");
			m5.MediaFileId = 5;
			m5.Rate = 3;

			return new[] { m1, m2, m3, m4, m5 };
		}

		public IOrderedEnumerable<IMediaFileModel> CreateSortedModels() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			return si.ApplySort(this.CreateModels(), false);
		}

		[Test]
		public void ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.FilePath);
			var si = sic.Create();
			si.ApplySort(this.CreateModels(), false).Select(x => x.MediaFileId).Is(2, 1, 3, 4, 5);
			si.ApplySort(this.CreateModels(), true).Select(x => x.MediaFileId).Is(5, 4, 3, 1, 2);
		}

		[Test]
		public void ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.FilePath, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplySort(this.CreateModels(), false).Select(x => x.MediaFileId).Is(5, 4, 3, 1, 2);
			si.ApplySort(this.CreateModels(), true).Select(x => x.MediaFileId).Is(2, 1, 3, 4, 5);
		}

		[Test]
		public void 追加ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.FilePath);
			var si = sic.Create();
			si.ApplyThenBySort(this.CreateSortedModels(), false).Select(x => x.MediaFileId).Is(1, 3, 4, 2, 5);
			si.ApplyThenBySort(this.CreateSortedModels(), true).Select(x => x.MediaFileId).Is(4, 3, 1, 5, 2);
		}

		[Test]
		public void 追加ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.FilePath, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplyThenBySort(this.CreateSortedModels(), false).Select(x => x.MediaFileId).Is(4, 3, 1, 5, 2);
			si.ApplyThenBySort(this.CreateSortedModels(), true).Select(x => x.MediaFileId).Is(1, 3, 4, 2, 5);
		}
	}
}
