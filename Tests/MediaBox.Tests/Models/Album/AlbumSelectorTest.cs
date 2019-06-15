using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
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
			var selector = new AlbumSelector("main");
			for (var i = 0; i < count; i++) {
				using (var album = new RegisteredAlbum(selector)) {
					album.Create();
					album.AlbumPath.Value = "/iphone";
					album.ReflectToDataBase();
					container.AddAlbum(album.AlbumId.Value);
				}
			}

			using (var selector2 = new AlbumSelector("main")) {
				selector2.AlbumList.Count.Is(count);
				selector2.AlbumList.Select(x => x.AlbumId.Value).Is(Enumerable.Range(1, count));
			}
		}

		[Test]
		public void カレントアルバム変更() {
			var selector = new AlbumSelector("main");
			using var album1 = new RegisteredAlbum(selector);
			album1.Title.Value = "album1";
			using var album2 = new RegisteredAlbum(selector);
			album2.Title.Value = "album2";
			using var selector2 = new AlbumSelector("main");
			selector2.SetAlbumToCurrent(album1);
			album1.Is(selector2.CurrentAlbum.Value);
			selector2.SetAlbumToCurrent(album2);
			album2.Is(selector2.CurrentAlbum.Value);

			this.States.AlbumStates.AlbumHistory.Select(x => x.Title).Is("album2", "album1");
		}

		[Test]
		public void フォルダアルバム() {
			using var selector = new AlbumSelector("main");
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
			using var selector = new AlbumSelector("main");
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
			var (r1, media1) = this.Register(this.TestFiles.Image1Jpg);
			var (r2, media2) = this.Register(this.TestFiles.Image2Jpg);
			var (r3, media3) = this.Register(this.TestFiles.Image3Jpg);
			var (r4, media4) = this.Register(this.TestFiles.Image4Png);
			r1.Rate = 4;
			r2.Rate = 3;
			r3.Rate = 5;
			r4.Rate = 1;
			this.DataBase.SaveChanges();

			var container = Get.Instance<AlbumContainer>();
			var selector = new AlbumSelector("main");
			using (var ra = new RegisteredAlbum(selector)) {
				ra.Create();
				ra.Title.Value = "title";
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1, media2, media3, media4 });
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			selector.SetAlbumToCurrent(selector.AlbumList.First());
			await this.WaitTaskCompleted(3000);
			using var album = selector.AlbumList.First();
			album.Items.Count.Is(4);

			(selector.FilterSetter as FilterDescriptionManager).AddCondition();
			await RxUtility.WaitPolling(() => (selector.FilterSetter as FilterDescriptionManager).FilteringConditions.Count == 1, 100, 1000);
			(selector.FilterSetter as FilterDescriptionManager).CurrentFilteringCondition.Value = (selector.FilterSetter as FilterDescriptionManager).FilteringConditions.First();
			(selector.FilterSetter as FilterDescriptionManager).CurrentFilteringCondition.Value.AddRateFilter(4);
			await RxUtility.WaitPolling(() => album.Items.Count != 4, 100, 50000);
			album.Items.Count.Is(2);
			var image1 = this.TestFiles.Image1Jpg;
			var image3 = this.TestFiles.Image3Jpg;
			image1.Rate = 4;
			image3.Rate = 5;
			album.Items.Check(image1, image3);
		}

		[Test]
		public async Task アルバム削除() {
			var container = Get.Instance<AlbumContainer>();
			var selector = new AlbumSelector("main");
			// 事前データ準備
			var (r1, media1) = this.Register(this.TestFiles.Image1Jpg);
			var (r2, media2) = this.Register(this.TestFiles.Image2Jpg);
			var (r3, media3) = this.Register(this.TestFiles.Image3Jpg);
			var (r4, media4) = this.Register(this.TestFiles.Image4Png);

			using (var ra = new RegisteredAlbum(selector)) {
				await Task.Delay(150);
				ra.Create();
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1 });
				ra.Directories.Add(@"C:\test1");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using (var ra = new RegisteredAlbum(selector)) {
				await Task.Delay(150);
				ra.Create();
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1, media2 });
				ra.Directories.Add(@"C:\test2");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using (var ra = new RegisteredAlbum(selector)) {
				await Task.Delay(150);
				ra.Create();
				ra.AlbumPath.Value = "/iphone/picture";
				ra.AddFiles(new[] { media1, media2, media3 });
				ra.Directories.Add(@"C:\test3");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using var selector2 = new AlbumSelector("main");
			selector2.AlbumList.Count.Is(3);

			var album3 = selector2.AlbumList[2];

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
				selector2.AlbumList.Any(x => x == album3).IsTrue();
			}
			// 削除実行
			selector2.DeleteAlbum(album3);

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
				selector2.AlbumList.Any(x => x == album3).IsFalse();
			}
		}

		[Test]
		public void 不要アルバムのDispose() {
			using var selector = new AlbumSelector("main");

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

			using var ra = new RegisteredAlbum(selector);
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
