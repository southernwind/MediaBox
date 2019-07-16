using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;

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
			lock (this.DataBase) {
				this.DataBase.Albums.Count().Is(0);
			}
			album1.AlbumId.Value.Is(0);

			// 作成するとDB登録される
			album1.Create();
			lock (this.DataBase) {
				this.DataBase.Albums.Count().Is(1);
			}
			album1.AlbumId.Value.Is(1);

			// 作成するとDB登録される
			album2.Create();
			lock (this.DataBase) {
				this.DataBase.Albums.Count().Is(2);
			}
			album2.AlbumId.Value.Is(2);
		}

		[Test]
		public void データベース情報の反映読み込み() {
			using var selector = new AlbumSelector("main");
			using (var album = new RegisteredAlbum(selector)) {
				album.Create();
				album.Title.Value = "タイトル";
				album.AlbumPath.Value = @"/picture/ff";
				album.Directories.Add(@"C:\test\");
				album.Directories.Add(@"C:\data\");
				album.ReflectToDataBase();
				album.AlbumId.Value.Is(1);
			}
			using (var album = new RegisteredAlbum(selector)) {
				album.LoadFromDataBase(1);
				album.Title.Value.Is("タイトル");
				album.AlbumPath.Value.Is(@"/picture/ff");
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
				lock (this.DataBase) {
					this.DataBase
					.AlbumMediaFiles
					.Include(x => x.MediaFile)
					.Where(x => x.AlbumId == 1)
					.Select(x => x.MediaFile)
					.Check(this._d1.Image1Jpg, this._d1.Image2Jpg);
				}

				album.RemoveFiles(new[] { image1 });
				album.Count.Value.Is(1);
				album.Items.Check(this._d1.Image2Jpg);
				lock (this.DataBase) {
					this.DataBase
						.AlbumMediaFiles
						.Include(x => x.MediaFile)
						.Where(x => x.AlbumId == 1)
						.Select(x => x.MediaFile)
						.Check(this._d1.Image2Jpg);
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
	}
}
