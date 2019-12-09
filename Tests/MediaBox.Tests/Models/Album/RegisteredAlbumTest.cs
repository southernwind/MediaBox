using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class RegisteredAlbumTest : ModelTestClassBase {
		private TestFiles _d1;
		private TestFiles _dsub;
		private TestFiles _d2;
		public override void SetUp() {
			base.SetUp();
			this.UseFileSystem();
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["1"], TestFileNames.Image1Jpg, TestFileNames.Image2Jpg, TestFileNames.Image3Jpg);
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["sub"], TestFileNames.Image4Png);
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["2"], TestFileNames.NoExifJpg);

			this._d1 = new TestFiles(this.TestDirectories["1"]);
			this._dsub = new TestFiles(this.TestDirectories["sub"]);
			this._d2 = new TestFiles(this.TestDirectories["2"]);

			this.Register(this._d1.Image1Jpg);
			this.Register(this._d1.Image2Jpg);
			this.Register(this._d1.Image3Jpg);
			this.Register(this._dsub.Image4Png);
			this.Register(this._d2.NoExifJpg);
		}

		[Test]
		public void アルバム作成() {
			using var selector = new AlbumSelector("main");
			using var album1 = new RegisteredAlbum(selector);
			using var album2 = new RegisteredAlbum(selector);
			// インスタンス生成時点では何も起きない
			lock (this.Rdb) {
				this.Rdb.Albums.Count().Is(0);
			}
			album1.AlbumId.Value.Is(0);

			// 作成するとDB登録される
			album1.Create();
			lock (this.Rdb) {
				this.Rdb.Albums.Count().Is(1);
			}
			album1.AlbumId.Value.Is(1);

			// 作成するとDB登録される
			album2.Create();
			lock (this.Rdb) {
				this.Rdb.Albums.Count().Is(2);
			}
			album2.AlbumId.Value.Is(2);
		}

		[Test]
		public void データベース情報の反映読み込み() {
			using var selector = new AlbumSelector("main");
			using (var album = new RegisteredAlbum(selector)) {
				album.Create();
				album.Title.Value = "タイトル";
				album.Directories.Add(@"C:\test\");
				album.Directories.Add(@"C:\data\");
				album.ReflectToDataBase();
				album.AlbumId.Value.Is(1);
			}
			using (var album = new RegisteredAlbum(selector)) {
				album.LoadFromDataBase(1);
				album.Title.Value.Is("タイトル");
				album.Directories.OrderBy(x => x).Is(@"C:\data\", @"C:\test\");
			}
		}

		[Test]
		public async Task ファイル追加削除() {
			using var selector = new AlbumSelector("main");
			using var image1 = this.MediaFactory.Create(this._d1.Image1Jpg.FilePath);
			using var image2 = this.MediaFactory.Create(this._d1.Image2Jpg.FilePath);

			using (var album = new RegisteredAlbum(selector)) {
				album.Create();
				album.AddFiles(new[] { image1, image2 });
				album.Count.Value.Is(2);
				album.Items.Check(this._d1.Image1Jpg, this._d1.Image2Jpg);
				{
					long[] ids;
					lock (this.Rdb) {
						ids = this.Rdb.AlbumMediaFiles.Where(x => x.AlbumId == 1).Select(x => x.MediaFileId).ToArray();
					}
					lock (this.Rdb) {
						this.DocumentDb
							.GetMediaFilesCollection()
							.Query()
							.Check(this._d1.Image1Jpg, this._d1.Image2Jpg);
					}
				}

				album.RemoveFiles(new[] { image1 });
				album.Count.Value.Is(1);
				album.Items.Check(this._d1.Image2Jpg);
				lock (this.Rdb) {

					long[] ids;
					lock (this.Rdb) {
						ids = this.Rdb.AlbumMediaFiles.Where(x => x.AlbumId == 1).Select(x => x.MediaFileId).ToArray();
					}
					lock (this.Rdb) {
						this.DocumentDb
							.GetMediaFilesCollection()
							.Query()
							.Check(this._d1.Image2Jpg);
					}
				}
			}

			using (var album = new RegisteredAlbum(selector)) {
				album.LoadFromDataBase(1);
				await this.WaitTaskCompleted(3000);
				album.Count.Value.Is(1);
				album.Items.Check(this._d1.Image2Jpg);
			}
		}

		[Test]
		public void ファイル追加例外() {
			using var selector = new AlbumSelector("main");
			using var album = new RegisteredAlbum(selector);
			Assert.Throws<ArgumentNullException>(() => {
				album.AddFiles(null);
			});
		}

		[Test]
		public void ファイル削除例外() {
			using var selector = new AlbumSelector("main");
			using var album = new RegisteredAlbum(selector);
			Assert.Throws<ArgumentNullException>(() => {
				album.RemoveFiles(null);
			});
		}

		[Test]
		public async Task ロードパターン1() {
			using var selector = new AlbumSelector("main");
			using var image1 = this.MediaFactory.Create(this._d1.Image1Jpg.FilePath);
			using var image2 = this.MediaFactory.Create(this._d1.Image2Jpg.FilePath);
			using (var album = new RegisteredAlbum(selector)) {
				album.Create();
				album.Directories.Add(this.TestDirectories["2"]);
				album.ReflectToDataBase();
				album.AddFiles(new[] { image1, image2 });
			}

			using var ra = new RegisteredAlbum(selector);
			ra.LoadFromDataBase(1);
			await this.WaitTaskCompleted(3000);
			ra.Items.Check(
				this._d1.Image1Jpg,
				this._d1.Image2Jpg,
				this._d2.NoExifJpg);
		}

		[Test]
		public async Task ロードパターン2() {
			using var selector = new AlbumSelector("main");
			using var image1 = this.MediaFactory.Create(this._d1.Image1Jpg.FilePath);
			using (var album = new RegisteredAlbum(selector)) {
				album.Create();
				album.Directories.Add(this.TestDirectories["2"]);
				album.Directories.Add(this.TestDirectories["sub"]);
				album.ReflectToDataBase();
				album.AddFiles(new[] { image1 });
			}

			using var ra = new RegisteredAlbum(selector);
			ra.LoadFromDataBase(1);
			await this.WaitTaskCompleted(3000);
			ra.Items.Check(
				this._d1.Image1Jpg,
				this._dsub.Image4Png,
				this._d2.NoExifJpg);
		}

		[Test]
		public async Task ロードパターン3() {
			using var selector = new AlbumSelector("main");
			using var image1 = this.MediaFactory.Create(this._d1.Image1Jpg.FilePath);
			using (var album = new RegisteredAlbum(selector)) {
				album.Create();
				album.Directories.Add(this.TestDirectories["2"]);
				album.Directories.Add(this.TestDirectories["sub"]);
				album.ReflectToDataBase();
				album.AddFiles(new[] { image1 });
			}

			using var ra = new RegisteredAlbum(selector);
			ra.LoadFromDataBase(1);
			await this.WaitTaskCompleted(3000);
			ra.Items.Check(
				this._d1.Image1Jpg,
				this._dsub.Image4Png,
				this._d2.NoExifJpg);
		}

		[Test]
		public async Task ファイル追加削除追従() {
			using var selector = new AlbumSelector("main");
			using var ra = new RegisteredAlbum(selector);
			ra.Directories.Add(this.TestDataDir);

			ra.Items.Count.Is(0);

			using var mfm = Get.Instance<MediaFileManager>();
			mfm.RegisterItems(new[] { this.TestFiles.Image1Jpg.FilePath });
			await this.WaitTaskCompleted(3000);
			ra.Items.Count.Is(1);
			ra.Items.Check(this.TestFiles.Image1Jpg);

			mfm.RegisterItems(new[] { this.TestFiles.Image3Jpg.FilePath, this.TestFiles.Video1Mov.FilePath });
			await this.WaitTaskCompleted(3000);
			ra.Items.Count.Is(3);
			ra.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image3Jpg, this.TestFiles.Video1Mov);

			mfm.DeleteItems(new[] { ra.Items.First(x => x.FilePath == this.TestFiles.Image3Jpg.FilePath) });
			ra.Items.Count.Is(2);
			ra.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Video1Mov);
		}
	}
}
