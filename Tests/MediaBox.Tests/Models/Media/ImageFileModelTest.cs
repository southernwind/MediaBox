using System.Linq;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	internal class ImageFileModelTest : MediaFileTest {
		[SetUp]
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

		protected override MediaFileModel GetInstance(string filePath) {
			return new ImageFileModel(filePath);
		}

		[Test]
		public void イメージ読み込み() {
			using var ifm = this.GetInstance(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			ifm.Image.IsNull();
			ifm.LoadImage();
			ifm.Image.IsNotNull();
		}

		[Test]
		public void イメージアンロード() {
			using var ifm = this.GetInstance(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			ifm.Image.IsNull();
			ifm.LoadImage();
			ifm.Image.IsNotNull();
			ifm.UnloadImage();
			ifm.Image.IsNull();
		}

		[Test]
		public override void サムネイル作成() {
			using var ifm = this.GetInstance(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			ifm.ThumbnailCreated.IsFalse();
			ifm.Thumbnail.IsNull();
			ifm.CreateThumbnail();
			ifm.ThumbnailCreated.IsTrue();
			ifm.Thumbnail.IsNotNull();
		}

		[Test]
		public override void データベース登録と読み込み() {
			using (var ifm = this.GetInstance(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel) {
				ifm.Rate = 4;
				this.DataBase.MediaFiles.Add(ifm.CreateDataBaseRecord());
				this.DataBase.SaveChanges();
			}

			var row = this
				.DataBase
				.MediaFiles
				.Include(x => x.MediaFileTags)
				.ThenInclude(x => x.Tag)
				.First();
			var test = this.TestFiles.Image1Jpg;
			test.Rate = 4;
			row.Check(test);

			using (var media = this.GetInstance(this.TestFiles.Image1Jpg.FilePath)) {
				media.LoadFromDataBase();
				media.Check(test, true, false);
			}

			using (var media = this.GetInstance(this.TestFiles.Image2Jpg.FilePath)) {
				media.LoadFromDataBase(row);
				media.Check(test, false, false);
			}
		}

		[Test]
		public override void データベース更新() {
			var record = new MediaFile();
			using (var media = this.GetInstance(this.TestFiles.Image1Jpg.FilePath)) {
				media.Rate = 4;
				media.UpdateDataBaseRecord(record);
			}

			record.MediaFileTags = new MediaFileTag[] { };
			var test = this.TestFiles.Image1Jpg;
			test.Rate = 4;
			record.Check(test);
		}
	}
}
