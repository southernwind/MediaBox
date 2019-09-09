using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class FolderAlbumTest : ModelTestClassBase {
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
		public async Task ロードパターン1() {
			using var selector = new AlbumSelector("main");
			using var fa = new FolderAlbum(this.TestDirectories["1"], selector);
			await this.WaitTaskCompleted(3000);
			fa.Title.Value.Is(this.TestDirectories["1"]);
			fa.DirectoryPath.Is(this.TestDirectories["1"]);
			fa.Items.Check(
				this._d1.Image1Jpg,
				this._d1.Image2Jpg,
				this._d1.Image3Jpg,
				this._dsub.Image4Png);
		}

		[Test]
		public async Task ロードパターン2() {
			using var selector = new AlbumSelector("main");
			using var fa = new FolderAlbum(this.TestDirectories["2"], selector);
			await this.WaitTaskCompleted(3000);
			fa.Title.Value.Is(this.TestDirectories["2"]);
			fa.DirectoryPath.Is(this.TestDirectories["2"]);
			fa.Items.Check(
				this._d2.NoExifJpg);
		}

		[Test]
		public async Task ロードパターンsub() {
			using var selector = new AlbumSelector("main");
			using var fa = new FolderAlbum(this.TestDirectories["sub"], selector);
			await this.WaitTaskCompleted(3000);
			fa.Title.Value.Is(this.TestDirectories["sub"]);
			fa.DirectoryPath.Is(this.TestDirectories["sub"]);
			fa.Items.Check(
				this._dsub.Image4Png);
		}

		[Test]
		public async Task ファイル追加削除追従() {
			using var selector = new AlbumSelector("main");
			using var fa = new FolderAlbum(this.TestDataDir, selector);
			fa.Items.Count.Is(0);

			using var mfm = Get.Instance<MediaFileManager>();
			mfm.RegisterItems(new[] { this.TestFiles.Image1Jpg.FilePath });
			await this.WaitTaskCompleted(3000);
			fa.Items.Count.Is(1);
			fa.Items.Check(this.TestFiles.Image1Jpg);

			mfm.RegisterItems(new[] { this.TestFiles.Image3Jpg.FilePath, this.TestFiles.Video1Mov.FilePath });
			await this.WaitTaskCompleted(3000);
			fa.Items.Count.Is(3);
			fa.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image3Jpg, this.TestFiles.Video1Mov);

			mfm.DeleteItems(new[] { fa.Items.First(x => x.FilePath == this.TestFiles.Image3Jpg.FilePath) });
			fa.Items.Count.Is(2);
			fa.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Video1Mov);
		}
	}
}
