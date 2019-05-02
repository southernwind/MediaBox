
using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class FolderAlbumTest : ModelTestClassBase {
		private TestFiles _d1;
		private TestFiles _dsub;
		private TestFiles _d2;
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
			this.UseFileSystem();

			FileUtility.Copy(this.TestDataDir, this.TestDirectories["1"], TestFileNames.Image1Jpg, TestFileNames.Image2Jpg, TestFileNames.Image3Jpg);
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["sub"], TestFileNames.Image4Png);
			FileUtility.Copy(this.TestDataDir, this.TestDirectories["2"], TestFileNames.NoExifJpg);

			this._d1 = new TestFiles(this.TestDirectories["1"]);
			this._dsub = new TestFiles(this.TestDirectories["sub"]);
			this._d2 = new TestFiles(this.TestDirectories["2"]);
			this.DataBase.MediaFiles.AddRange(
				this.MediaFactory.Create(this._d1.Image1Jpg.FilePath).CreateDataBaseRecord(),
				this.MediaFactory.Create(this._d1.Image2Jpg.FilePath).CreateDataBaseRecord(),
				this.MediaFactory.Create(this._d1.Image3Jpg.FilePath).CreateDataBaseRecord(),
				this.MediaFactory.Create(this._dsub.Image4Png.FilePath).CreateDataBaseRecord(),
				this.MediaFactory.Create(this._d2.NoExifJpg.FilePath).CreateDataBaseRecord()
			);
			this.DataBase.SaveChanges();
		}

		[Test]
		public void ロードパターン1() {
			var fa = new FolderAlbum(this.TestDirectories["1"]);
			fa.Title.Value.Is(this.TestDirectories["1"]);
			fa.DirectoryPath.Is(this.TestDirectories["1"]);
			fa.Items.Check(
				this._d1.Image1Jpg,
				this._d1.Image2Jpg,
				this._d1.Image3Jpg,
				this._dsub.Image4Png);
		}

		[Test]
		public void ロードパターン2() {
			var fa = new FolderAlbum(this.TestDirectories["2"]);
			fa.Title.Value.Is(this.TestDirectories["2"]);
			fa.DirectoryPath.Is(this.TestDirectories["2"]);
			fa.Items.Check(
				this._d2.NoExifJpg);
		}

		[Test]
		public void ロードパターンsub() {
			var fa = new FolderAlbum(this.TestDirectories["sub"]);
			fa.Title.Value.Is(this.TestDirectories["sub"]);
			fa.DirectoryPath.Is(this.TestDirectories["sub"]);
			fa.Items.Check(
				this._dsub.Image4Png);
		}
	}
}
