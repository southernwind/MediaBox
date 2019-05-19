using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumSelectorTest : ModelTestClassBase {
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void アルバムリスト(int count) {
			var container = Get.Instance<AlbumContainer>();
			for (var i = 0; i < count; i++) {
				using (var album = new RegisteredAlbum(this.Filter, this.Sort)) {
					album.Create();
					album.AlbumPath.Value = "/iphone";
					album.ReflectToDataBase();
					container.AddAlbum(album.AlbumId.Value);
				}
			}

			using (var selector = new AlbumSelector()) {
				selector.AlbumList.Count.Is(count);
				selector.AlbumList.Select(x => x.AlbumId.Value).Is(Enumerable.Range(1, count));
			}
		}

		[Test]
		public void カレントアルバム変更() {
			using var album1 = new RegisteredAlbum(this.Filter, this.Sort);
			album1.Title.Value = "album1";
			using var album2 = new RegisteredAlbum(this.Filter, this.Sort);
			album2.Title.Value = "album2";
			using var selector = new AlbumSelector();
			selector.SetAlbumToCurrent(album1);
			album1.Is(selector.CurrentAlbum.Value);
			selector.SetAlbumToCurrent(album2);
			album2.Is(selector.CurrentAlbum.Value);

			this.States.AlbumStates.AlbumHistory.Select(x => x.Title).Is("album2", "album1");
		}

		[Test]
		public void フォルダアルバム() {
			using var selector = new AlbumSelector();
			selector.SetFolderAlbumToCurrent();
			selector.CurrentAlbum.Value.IsNull();

			selector.FolderAlbumPath.Value = @"C:\test\picture\";
			selector.CurrentAlbum.Value.IsNull();
			selector.SetFolderAlbumToCurrent();
			((FolderAlbum)selector.CurrentAlbum.Value).DirectoryPath.Is(@"C:\test\picture\");
			selector.FolderAlbumPath.Value = @"D:\test\";
			selector.SetFolderAlbumToCurrent();
			((FolderAlbum)selector.CurrentAlbum.Value).DirectoryPath.Is(@"D:\test\");

			this.States.AlbumStates.AlbumHistory.Select(x => x.Title).Is(@"D:\test\", @"C:\test\picture\");
		}

		[Test]
		public void データベース検索アルバム() {
			using var selector = new AlbumSelector();
			selector.SetDatabaseAlbumToCurrent("tag:AAA", "AAA");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is("tag:AAA");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).TagName.Is(@"AAA");
			selector.SetDatabaseAlbumToCurrent("tag:CCC", "CCC");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is(@"tag:CCC");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).TagName.Is(@"CCC");

			this.States.AlbumStates.AlbumHistory.Select(x => x.Title).Is("tag:CCC", "tag:AAA");
		}

		[Test]
		public async Task フィルター変更() {
			// 事前データ準備
			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			media1.Rate = 4;
			media2.Rate = 3;
			media3.Rate = 5;
			media4.Rate = 1;
			var r1 = media1.CreateDataBaseRecord();
			var r2 = media2.CreateDataBaseRecord();
			var r3 = media3.CreateDataBaseRecord();
			var r4 = media4.CreateDataBaseRecord();
			this.DataBase.MediaFiles.AddRange(r1, r2, r3, r4);
			this.DataBase.SaveChanges();
			media1.MediaFileId = r1.MediaFileId;
			media2.MediaFileId = r2.MediaFileId;
			media3.MediaFileId = r3.MediaFileId;
			media4.MediaFileId = r4.MediaFileId;
			this.DataBase.SaveChanges();

			var container = Get.Instance<AlbumContainer>();
			var selector = new AlbumSelector();
			using (var ra = new RegisteredAlbum(selector.FilterDescriptionManager, selector.SortDescriptionManager)) {
				ra.Create();
				ra.Title.Value = "title";
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1, media2, media3, media4 });
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			selector.SetAlbumToCurrent(selector.AlbumList.First());

			using var album = selector.AlbumList.First();
			album.Items.Count.Is(4);

			selector.FilterDescriptionManager.AddCondition();
			await RxUtility.WaitPolling(() => selector.FilterDescriptionManager.FilteringConditions.Count == 1, 100, 1000);
			selector.FilterDescriptionManager.CurrentFilteringCondition.Value = selector.FilterDescriptionManager.FilteringConditions.First();
			selector.FilterDescriptionManager.CurrentFilteringCondition.Value.AddRateFilter(4);
			await RxUtility.WaitPolling(() => album.Items.Count != 4, 100, 50000);
			album.Items.Count.Is(2);
			var image1 = this.TestFiles.Image1Jpg;
			var image3 = this.TestFiles.Image3Jpg;
			image1.Rate = 4;
			image3.Rate = 5;
			album.Items.Check(image1, image3);
		}

		[Test]
		public void アルバム削除() {
			var container = Get.Instance<AlbumContainer>();
			// 事前データ準備
			using var media1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var media2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var media3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var media4 = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);

			var r1 = media1.CreateDataBaseRecord();
			var r2 = media2.CreateDataBaseRecord();
			var r3 = media3.CreateDataBaseRecord();
			var r4 = media4.CreateDataBaseRecord();
			this.DataBase.MediaFiles.AddRange(r1, r2, r3, r4);
			this.DataBase.SaveChanges();
			media1.MediaFileId = r1.MediaFileId;
			media2.MediaFileId = r2.MediaFileId;
			media3.MediaFileId = r3.MediaFileId;
			media4.MediaFileId = r4.MediaFileId;

			using (var ra = new RegisteredAlbum(this.Filter, this.Sort)) {
				ra.Create();
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1 });
				ra.Directories.Add(@"C:\test1");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using (var ra = new RegisteredAlbum(this.Filter, this.Sort)) {
				ra.Create();
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1, media2 });
				ra.Directories.Add(@"C:\test2");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using (var ra = new RegisteredAlbum(this.Filter, this.Sort)) {
				ra.Create();
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1, media2, media3 });
				ra.Directories.Add(@"C:\test3");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using var selector = Get.Instance<AlbumSelector>();
			selector.AlbumList.Count.Is(3);

			var album3 = selector.AlbumList[2];

			// 実行前状態
			lock (this.DataBase) {
				this.DataBase.Albums
					.Include(x => x.AlbumMediaFiles)
					.ThenInclude(x => x.MediaFile)
					.Include(x => x.AlbumDirectories)
					.Count(x => x.AlbumId == 3)
					.Is(1);
				this.DataBase.AlbumDirectories.Count().Is(3);
				this.DataBase.AlbumMediaFiles.Count().Is(6);
				this.DataBase.Albums.Count().Is(3);
				selector.AlbumList.Any(x => x == album3).IsTrue();
			}
			// 削除実行
			selector.DeleteAlbum(album3);

			// 実行後状態
			lock (this.DataBase) {
				this.DataBase.Albums
					.Include(x => x.AlbumMediaFiles)
					.ThenInclude(x => x.MediaFile)
					.Include(x => x.AlbumDirectories)
					.Count(x => x.AlbumId == 3)
					.Is(0);
				this.DataBase.AlbumDirectories.Count().Is(2);
				this.DataBase.AlbumMediaFiles.Count().Is(3);
				this.DataBase.Albums.Count().Is(2);
				selector.AlbumList.Any(x => x == album3).IsFalse();
			}
		}

		[Test]
		public void 不要アルバムのDispose() {
			using var selector = new AlbumSelector();

			selector.FolderAlbumPath.Value = @"C:\test\picture\";
			selector.SetFolderAlbumToCurrent();
			var fa = selector.CurrentAlbum.Value as FolderAlbum;
			fa.IsNotNull();
			fa.Disposed.IsFalse();

			selector.SetDatabaseAlbumToCurrent("tag:AAA", "AAA");
			fa.Disposed.IsTrue();
			var da = selector.CurrentAlbum.Value as LookupDatabaseAlbum;
			da.IsNotNull();
			da.Disposed.IsFalse();

			using var ra = new RegisteredAlbum(this.Filter, this.Sort);
			ra.Title.Value = "album1";
			selector.SetAlbumToCurrent(ra);
			da.Disposed.IsTrue();
			ra.Disposed.IsFalse();

			selector.FolderAlbumPath.Value = @"C:\test\picture\";
			selector.SetFolderAlbumToCurrent();
			ra.Disposed.IsFalse();
		}
	}
}
