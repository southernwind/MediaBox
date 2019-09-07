using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Tests.Models.Album.Sort.SortItemCreators {
	internal class CreationTime : ModelTestClassBase {

		public IEnumerable<IMediaFileModel> CreateModels() {
			var m1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			m1.MediaFileId = 1;
			m1.Rate = 2;
			m1.CreationTime = new DateTime(2019, 10, 1, 0, 0, 0);
			var m2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			m2.MediaFileId = 2;
			m2.Rate = 3;
			m2.CreationTime = new DateTime(2022, 8, 1, 0, 0, 0);
			var m3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			m3.MediaFileId = 3;
			m3.Rate = 2;
			m3.CreationTime = new DateTime(2014, 6, 30, 11, 5, 30);
			var m4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			m4.MediaFileId = 4;
			m4.Rate = 2;
			m4.CreationTime = new DateTime(2014, 6, 30, 11, 5, 31);
			var m5 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);
			m5.MediaFileId = 5;
			m5.Rate = 3;
			m5.CreationTime = new DateTime(2014, 6, 4, 0, 0, 0);

			return new[] { m1, m2, m3, m4, m5 };
		}

		public IOrderedEnumerable<IMediaFileModel> CreateSortedModels() {
			var sic = new SortItemCreator(SortItemKeys.Rate);
			var si = sic.Create();
			return si.ApplySort(this.CreateModels(), false);
		}

		[Test]
		public void ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.CreationTime);
			var si = sic.Create();
			si.ApplySort(this.CreateModels(), false).Select(x => x.MediaFileId).Is(5, 3, 4, 1, 2);
			si.ApplySort(this.CreateModels(), true).Select(x => x.MediaFileId).Is(2, 1, 4, 3, 5);
		}

		[Test]
		public void ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.CreationTime, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplySort(this.CreateModels(), false).Select(x => x.MediaFileId).Is(2, 1, 4, 3, 5);
			si.ApplySort(this.CreateModels(), true).Select(x => x.MediaFileId).Is(5, 3, 4, 1, 2);
		}

		[Test]
		public void 追加ソートテスト昇順() {
			var sic = new SortItemCreator(SortItemKeys.CreationTime);
			var si = sic.Create();
			si.ApplyThenBySort(this.CreateSortedModels(), false).Select(x => x.MediaFileId).Is(3, 4, 1, 5, 2);
			si.ApplyThenBySort(this.CreateSortedModels(), true).Select(x => x.MediaFileId).Is(1, 4, 3, 2, 5);
		}

		[Test]
		public void 追加ソートテスト降順() {
			var sic = new SortItemCreator(SortItemKeys.CreationTime, ListSortDirection.Descending);
			var si = sic.Create();
			si.ApplyThenBySort(this.CreateSortedModels(), false).Select(x => x.MediaFileId).Is(1, 4, 3, 2, 5);
			si.ApplyThenBySort(this.CreateSortedModels(), true).Select(x => x.MediaFileId).Is(3, 4, 1, 5, 2);
		}
	}
}
