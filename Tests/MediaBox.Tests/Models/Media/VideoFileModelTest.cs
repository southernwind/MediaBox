﻿using System.Linq;

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
			ifm.ThumbnailCreated.IsFalse();
			ifm.Thumbnail.IsNull();
			ifm.CreateThumbnail();
			ifm.ThumbnailCreated.IsTrue();
			ifm.Thumbnail.IsNotNull();
		}

		[Test]
		public override void データベース登録と読み込み() {
			using (var ifm = this.GetInstance(this.TestFiles.Video1Mov.FilePath) as VideoFileModel) {
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
			var test = this.TestFiles.Video1Mov;
			test.Rate = 4;
			row.Check(test);

			using (var media = this.GetInstance(this.TestFiles.Video1Mov.FilePath) as VideoFileModel) {
				media.LoadFromDataBase();
				media.Check(test, true, false);
				media.Duration.Is(0.083333);
				media.Rotation.Is(-90);
			}

			using (var media = this.GetInstance(TestFileNames.NotExistsFileMov) as VideoFileModel) {
				media.LoadFromDataBase(row);
				media.Check(test, false, false);
				media.Duration.Is(0.083333);
				media.Rotation.Is(-90);
			}
		}

		[Test]
		public override void データベース更新() {
			var record = new MediaFile();
			using (var media = this.GetInstance(this.TestFiles.Video1Mov.FilePath) as VideoFileModel) {
				media.Rate = 4;
				media.UpdateDataBaseRecord(record);
			}

			record.MediaFileTags = new MediaFileTag[] { };
			var test = this.TestFiles.Video1Mov;
			test.Rate = 4;
			record.Check(test);
		}
	}
}
