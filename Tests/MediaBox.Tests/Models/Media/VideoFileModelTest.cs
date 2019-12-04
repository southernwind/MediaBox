using System.Linq;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;

namespace SandBeige.MediaBox.Tests.Models.Media {
	internal class VideoFileModelTest : MediaFileTest {
		protected override MediaFileModel GetInstance(string filePath) {
			return new VideoFileModel(filePath);
		}

		[Test]
		public override void サムネイル作成() {
			using var ifm = this.GetInstance(this.TestFiles.Video1Mov.FilePath) as VideoFileModel;
			ifm.Resolution = this.TestFiles.Video1Mov.Resolution;
			ifm.ThumbnailCreated.IsFalse();
			ifm.ThumbnailFilePath.IsNull();
			ifm.CreateThumbnail();
			ifm.ThumbnailCreated.IsTrue();
			ifm.ThumbnailFilePath.IsNotNull();
		}

		[Test]
		public override void データベース登録と読み込み() {
			var (r, _) = this.Register(this.TestFiles.Video1Mov);
			r.Rate = 4;
			this.DocumentDb.GetMediaFilesCollection().Update(r);

			var row = this
				.DocumentDb
				.GetMediaFilesCollection()
				.Query()
				.First();
			var test = this.TestFiles.Video1Mov;
			test.Rate = 4;
			row.Check(test);

			using (var media = this.GetInstance(this.TestFiles.Video1Mov.FilePath) as VideoFileModel) {
				media.LoadFromDataBase();
				media.Check(test, true, false);
				media.Duration.Is(0.083333);
				media.Rotation.Is(90);
			}

			using (var media = this.GetInstance(TestFileNames.NotExistsFileMov) as VideoFileModel) {
				media.LoadFromDataBase(row);
				media.Check(test, false, false);
				media.Duration.Is(0.083333);
				media.Rotation.Is(90);
			}
		}

		[Test]
		public override void データベース更新() {
			var record = new MediaFile();
			using (var media = this.GetInstance(this.TestFiles.Video1Mov.FilePath) as VideoFileModel) {
				media.Rate = 4;
				media.UpdateDataBaseRecord(record);
			}

			record.Tags = new string[] { };
			var test = this.TestFiles.Video1Mov;
			test.Rate = 4;
			record.Check(test);
		}

		[Test]
		public void 特殊ファイル名() {
			var record = new MediaFile();
			using var media = this.GetInstance(this.TestFiles.SpecialFileNameVideoMov.FilePath) as VideoFileModel;
			media.UpdateDataBaseRecord(record);

			media.Check(this.TestFiles.SpecialFileNameVideoMov);
			record.Tags = new string[] { };
			record.Check(this.TestFiles.SpecialFileNameVideoMov);
		}
	}
}
