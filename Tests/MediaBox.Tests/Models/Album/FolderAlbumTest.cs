﻿
using System.Threading.Tasks;

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
			var selector = new AlbumSelector("main");
			var fa = new FolderAlbum(this.TestDirectories["1"], selector);
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
			var selector = new AlbumSelector("main");
			var fa = new FolderAlbum(this.TestDirectories["2"], selector);
			await this.WaitTaskCompleted(3000);
			fa.Title.Value.Is(this.TestDirectories["2"]);
			fa.DirectoryPath.Is(this.TestDirectories["2"]);
			fa.Items.Check(
				this._d2.NoExifJpg);
		}

		[Test]
		public async Task ロードパターンsub() {
			var selector = new AlbumSelector("main");
			var fa = new FolderAlbum(this.TestDirectories["sub"], selector);
			await this.WaitTaskCompleted(3000);
			fa.Title.Value.Is(this.TestDirectories["sub"]);
			fa.DirectoryPath.Is(this.TestDirectories["sub"]);
			fa.Items.Check(
				this._dsub.Image4Png);
		}
	}
}
