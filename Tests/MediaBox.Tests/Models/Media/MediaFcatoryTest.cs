using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	internal class MediaFcatory : ModelTestClassBase {

		[Test]
		public void 動画ファイル() {
			var mov1 = this.MediaFactory.Create(this.TestFiles.Video1Mov.FilePath);
			mov1.CreateDataBaseRecord();
			(mov1 as VideoFileModel).IsNotNull();
			mov1.Check(this.TestFiles.Video1Mov);
		}

		[Test]
		public void 画像ファイル() {
			var jpg1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			jpg1.CreateDataBaseRecord();
			(jpg1 as ImageFileModel).IsNotNull();
			jpg1.Check(this.TestFiles.Image1Jpg);
		}

		[Test]
		public void 同一ファイルパス同一インスタンス() {
			var jpg11 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var jpg12 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var jpg21 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var jpg22 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var jpg13 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			jpg11.Is(jpg12);
			jpg11.Is(jpg13);
			jpg21.Is(jpg22);
			jpg11.IsNot(jpg21);
		}

		[Test]
		public void Disposeでプールしていたインスタンスを破棄() {
			var jpg11 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var jpg12 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			jpg11.Dispose();
			var jpg13 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			jpg11.Is(jpg12);
			jpg11.IsNot(jpg13);
		}
	}
}
