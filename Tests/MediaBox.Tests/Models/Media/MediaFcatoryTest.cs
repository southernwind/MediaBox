using NUnit.Framework;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	internal class MediaFcatory : ModelTestClassBase {
		[SetUp]
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

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
			var jpg1_1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var jpg1_2 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var jpg2_1 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var jpg2_2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var jpg1_3 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			jpg1_1.Is(jpg1_2);
			jpg1_1.Is(jpg1_3);
			jpg2_1.Is(jpg2_2);
			jpg1_1.IsNot(jpg2_1);
		}

		[Test]
		public void Disposeでプールしていたインスタンスを破棄() {
			var jpg1_1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var jpg1_2 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			jpg1_1.Dispose();
			var jpg1_3 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			jpg1_1.Is(jpg1_2);
			jpg1_1.IsNot(jpg1_3);
		}
	}
}
