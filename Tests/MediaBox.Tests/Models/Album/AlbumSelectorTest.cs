using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumSelectorTest : ModelTestClassBase {
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void アルバムリスト(int count) {
			using var container = Get.Instance<AlbumContainer>();
			using var selector = new AlbumSelector("main");
			for (var i = 0; i < count; i++) {
				using var album = new RegisteredAlbum(selector);
				album.Create();
				album.ReflectToDataBase();
				container.AddAlbum(album.AlbumId.Value);
			}

			using var selector2 = new AlbumSelector("main");
			selector2.AlbumList.Count.Is(count);
			selector2.AlbumList.Select(x => x.AlbumId.Value).Is(Enumerable.Range(1, count));
		}

		[Test]
		public void カレントアルバム変更() {
			using var selector = new AlbumSelector("main");
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
		public void タグ検索アルバム() {
			using var selector = new AlbumSelector("main");
			selector.SetDatabaseAlbumToCurrent("AAA");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is("タグ : AAA");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).TagName.Is(@"AAA");
			selector.SetDatabaseAlbumToCurrent("CCC");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is(@"タグ : CCC");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).TagName.Is(@"CCC");

			this.States.AlbumStates.AlbumHistory.Select(x => x.Title).Is("タグ : CCC", "タグ : AAA");
		}

		[Test]
		public void ワード検索アルバム() {
			using var selector = new AlbumSelector("main");
			selector.SetWordSearchAlbumToCurrent("AAA");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is("検索ワード : AAA");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Word.Is(@"AAA");
			selector.SetWordSearchAlbumToCurrent("CCC");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is(@"検索ワード : CCC");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Word.Is(@"CCC");

			this.States.AlbumStates.AlbumHistory.Select(x => x.Title).Is("検索ワード : CCC", "検索ワード : AAA");
		}

		[Test]
		public void 場所検索アルバム() {
			using var selector = new AlbumSelector("main");
			var address = new Address(new[] {
				new Position { Addresses = new[] {
					new PositionAddress {SequenceNumber = 1,Type = "prefecture",Name = "東京都"},
					new PositionAddress {SequenceNumber = 2,Type = "suburb",Name = "渋谷区"}
				}.ToList()},
				new Position { Addresses = new[] {
					new PositionAddress {SequenceNumber = 1,Type = "prefecture",Name = "大阪府"},
					new PositionAddress {SequenceNumber = 2,Type = "city",Name = "大阪市"},
					new PositionAddress {SequenceNumber = 3,Type = "suburb",Name = "此花区"}
				}.ToList()}
			});
			selector.SetPositionSearchAlbumToCurrent(address.Children[0]);
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is("場所 : 渋谷区");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Address.Is(address.Children[0]);
			selector.SetPositionSearchAlbumToCurrent(address.Children[1]);
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Title.Value.Is(@"場所 : 此花区");
			((LookupDatabaseAlbum)selector.CurrentAlbum.Value).Address.Is(address.Children[1]);

			this.States.AlbumStates.AlbumHistory.Select(x => x.Title).Is("場所 : 此花区", "場所 : 渋谷区");
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
			lock (this.DataBase) {
				this.DataBase.SaveChanges();
			}

			using var container = Get.Instance<AlbumContainer>();
			using var selector = new AlbumSelector("main");
			using (var ra = new RegisteredAlbum(selector)) {
				ra.Create();
				ra.Title.Value = "title";
				ra.AddFiles(new[] { media1, media2, media3, media4 });
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			selector.SetAlbumToCurrent(selector.AlbumList.First());
			await this.WaitTaskCompleted(3000);
			using var album = selector.AlbumList.First();
			album.Items.Count.Is(4);

			var fs = selector.FilterSetter.IsInstanceOf<FilterDescriptionManager>();
			fs.AddCondition();
			fs.CurrentFilteringCondition.Value = fs.FilteringConditions.First();
			fs.CurrentFilteringCondition.Value.AddRateFilter(4, SearchTypeComparison.GreaterThanOrEqual);
			await this.WaitTaskCompleted(3000);
			album.Items.Count.Is(2);
			var image1 = this.TestFiles.Image1Jpg;
			var image3 = this.TestFiles.Image3Jpg;
			image1.Rate = 4;
			image3.Rate = 5;
			album.Items.Check(image1, image3);
		}

		[Test]
		public void アルバム削除() {
			using var container = Get.Instance<AlbumContainer>();
			using var selector = new AlbumSelector("main");
			// 事前データ準備
			var (r1, media1) = this.Register(this.TestFiles.Image1Jpg);
			var (r2, media2) = this.Register(this.TestFiles.Image2Jpg);
			var (r3, media3) = this.Register(this.TestFiles.Image3Jpg);
			var (r4, media4) = this.Register(this.TestFiles.Image4Png);

			using (var ra = new RegisteredAlbum(selector)) {
				ra.Create();
				ra.AddFiles(new[] { media1 });
				ra.Directories.Add(@"C:\test1");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using (var ra = new RegisteredAlbum(selector)) {
				ra.Create();
				ra.AddFiles(new[] { media1, media2 });
				ra.Directories.Add(@"C:\test2");
				ra.ReflectToDataBase();
				container.AddAlbum(ra.AlbumId.Value);
			}

			using (var ra = new RegisteredAlbum(selector)) {
				ra.Create();
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
					.Include(x => x.AlbumScanDirectories)
					.Count(x => x.AlbumId == 3)
					.Is(1);
				this.DataBase.AlbumScanDirectories.Count().Is(3);
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
					.Include(x => x.AlbumScanDirectories)
					.Count(x => x.AlbumId == 3)
					.Is(0);
				this.DataBase.AlbumScanDirectories.Count().Is(2);
				this.DataBase.AlbumMediaFiles.Count().Is(3);
				this.DataBase.Albums.Count().Is(2);
				selector2.AlbumList.Any(x => x == album3).IsFalse();
			}
		}

		[Test]
		public void フォルダ() {
			// 事前データ準備
			this.Register(new TestFile { FilePath = @"C:\horse\jack.jpg" });
			this.Register(new TestFile { FilePath = @"C:\fish\shellfish\shrimp.jpg" });
			this.Register(new TestFile { FilePath = @"D:\test\ox.jpg" });
			this.Register(new TestFile { FilePath = @"C:\fish\t.jpg" });

			using var selector = new AlbumSelector("main");
			var root = selector.Folder.Value;
			var drives = root.Children.OrderBy(x => x.DisplayName).ToArray();
			drives.Length.Is(2);
			drives.Select(x => x.DisplayName).Is("C:(3)", "D:(1)");

			var c = drives[0];
			var d = drives[1];

			var c2 = c.Children.OrderBy(x => x.DisplayName).ToArray();
			c2.Length.Is(2);
			c2.Select(x => x.DisplayName).Is("fish(2)", "horse(1)");

			var fish = c2[0];
			var horse = c2[1];

			var fish2 = fish.Children.OrderBy(x => x.DisplayName).ToArray();
			fish2.Length.Is(1);
			fish2.Select(x => x.DisplayName).Is("shellfish(1)");
			horse.Children.Is();

			var d2 = d.Children.OrderBy(x => x.DisplayName).ToArray();
			d2.Length.Is(1);
			d2.Select(x => x.DisplayName).Is("test(1)");

			var test = d2[0];
			test.Children.Is();
		}

		[Test]
		public void 登録アルバム以外のアルバム削除() {
			using var selector = new AlbumSelector("main");
			selector.SetWordSearchAlbumToCurrent("aa");
			Assert.Throws<ArgumentException>(() => {
				selector.DeleteAlbum(selector.CurrentAlbum.Value);
			});
		}

		[Test]
		public void プロパティ() {
			using var selector = new AlbumSelector("main");
			selector.FilterSetter.IsInstanceOf<FilterDescriptionManager>();
			selector.SortSetter.IsInstanceOf<SortDescriptionManager>();
		}

		[Test]
		public void 不要アルバムのDispose() {
			using var selector = new AlbumSelector("main");

			selector.FolderAlbumPath.Value = @"C:\test\picture\";
			selector.SetFolderAlbumToCurrent();
			var fa = selector.CurrentAlbum.Value as FolderAlbum;
			fa.IsNotNull();
			fa.DisposeState.Is(DisposeState.NotDisposed);

			selector.SetDatabaseAlbumToCurrent("AAA");
			fa.DisposeState.Is(DisposeState.Disposed);
			var da = selector.CurrentAlbum.Value as LookupDatabaseAlbum;
			da.IsNotNull();
			da.DisposeState.Is(DisposeState.NotDisposed);

			using var ra = new RegisteredAlbum(selector);
			ra.Title.Value = "album1";
			selector.SetAlbumToCurrent(ra);
			da.DisposeState.Is(DisposeState.Disposed);
			ra.DisposeState.Is(DisposeState.NotDisposed);

			selector.FolderAlbumPath.Value = @"C:\test\picture\";
			selector.SetFolderAlbumToCurrent();
			ra.DisposeState.Is(DisposeState.NotDisposed);
		}
	}
}
