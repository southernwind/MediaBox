﻿using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileTest : ModelTestClassBase {
		[SetUp]
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

		[Test]
		public void インスタンス作成() {
			using var media = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			media.FilePath.Is(this.TestFiles.Image1Jpg.FilePath);
			media.FileName.Is(this.TestFiles.Image1Jpg.FileName);
			media.Extension.Is(this.TestFiles.Image1Jpg.Extension);
			media.MediaFileId.IsNull();
		}

		[Test]
		public void サムネイル作成() {
			using var media1 = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			media1.ThumbnailCreated.IsFalse();
			media1.CreateThumbnailIfNotExists();
			media1.ThumbnailCreated.IsTrue();

			using var media2 = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			media2.ThumbnailCreated.IsFalse();
			media2.CreateThumbnailIfNotExists();
			media2.ThumbnailCreated.IsTrue();

		}

		[Test]
		public void データベース登録と読み込み() {
			using (var media = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath)) {
				media.Thumbnail = new Thumbnail("absolutely");
				media.Location = new GpsLocation(50.1, 50.2, 50.3);
				media.Rate = 4;
				media.Resolution = new ComparableSize(52, 53);
				this.DataBase.MediaFiles.Add(media.CreateDataBaseRecord());
				this.DataBase.SaveChanges();
			}

			var row = this.DataBase.MediaFiles.First();
			row.ThumbnailFileName = "absolutely";
			row.FilePath.Is(this.TestFiles.Image1Jpg.FilePath);
			row.DirectoryPath.Is(Path.GetDirectoryName(this.TestFiles.Image1Jpg.FilePath) + @"\");
			row.Latitude.Is(50.1);
			row.Longitude.Is(50.2);
			row.Altitude.Is(50.3);
			row.FileSize.Is(this.TestFiles.Image1Jpg.FileSize);
			row.Rate.Is(4);
			row.Width.Is(52);
			row.Height.Is(53);

			using (var media = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath)) {
				media.Thumbnail.IsNull();
				media.Location.IsNull();
				media.FileSize.Is(0);
				media.Rate.Is(0);
				media.Resolution.IsNull();
				media.LoadFromDataBase();
				media.Thumbnail.FileName.Is("absolutely");
				media.Location.Is(new GpsLocation(50.1, 50.2, 50.3));
				media.FileSize.Is(this.TestFiles.Image1Jpg.FileSize);
				media.Rate.Is(4);
				media.Resolution.Is(new ComparableSize(52, 53));
			}

			using (var media = new MediaFileModelImpl(this.TestFiles.Image2Jpg.FilePath)) {
				media.Thumbnail.IsNull();
				media.Location.IsNull();
				media.FileSize.Is(0);
				media.Rate.Is(0);
				media.Resolution.IsNull();
				media.LoadFromDataBase(row);
				media.Thumbnail.FileName.Is("absolutely");
				media.Location.Is(new GpsLocation(50.1, 50.2, 50.3));
				media.FileSize.Is(this.TestFiles.Image1Jpg.FileSize);
				media.Rate.Is(4);
				media.Resolution.Is(new ComparableSize(52, 53));
			}
		}


		[Test]
		public void データベース更新() {
			var record = new MediaFile();
			using (var media = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath)) {
				media.Thumbnail = new Thumbnail("absolutely");
				media.Location = new GpsLocation(50.1, 50.2, 50.3);
				media.Rate = 4;
				media.Resolution = new ComparableSize(52, 53);
				media.UpdateDataBaseRecord(record);
			}

			record.ThumbnailFileName = "absolutely";
			record.FilePath.Is(this.TestFiles.Image1Jpg.FilePath);
			record.DirectoryPath.Is(Path.GetDirectoryName(this.TestFiles.Image1Jpg.FilePath) + @"\");
			record.Latitude.Is(50.1);
			record.Longitude.Is(50.2);
			record.Altitude.Is(50.3);
			record.FileSize.Is(this.TestFiles.Image1Jpg.FileSize);
			record.Rate.Is(4);
			record.Width.Is(52);
			record.Height.Is(53);
		}

		[Test]
		public void ファイル情報読み込み() {
			using var media = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			media.Exists.Is(true);
			media.CreationTime.Is(default(DateTime));
			media.ModifiedTime.Is(default(DateTime));
			media.LastAccessTime.Is(default(DateTime));
			media.FileSize.Is(0);
			media.UpdateFileInfo();
			media.Exists.Is(this.TestFiles.Image1Jpg.Exists);
			media.CreationTime.Is(this.TestFiles.Image1Jpg.CreationTime);
			media.ModifiedTime.Is(this.TestFiles.Image1Jpg.ModifiedTime);
			media.LastAccessTime.Is(this.TestFiles.Image1Jpg.LastAccessTime);
			media.FileSize.Is(this.TestFiles.Image1Jpg.FileSize);
		}

		[Test]
		public void タグ追加削除() {
			using var media = new MediaFileModelImpl(this.TestFiles.Image1Jpg.FilePath);
			media.Tags.Is();
			media.AddTag("foolish");
			media.Tags.Is("foolish");
			media.AddTag("weather");
			media.Tags.Is("foolish", "weather");
			media.RemoveTag("foo___h");
			media.Tags.Is("foolish", "weather");
			media.RemoveTag("foolish");
			media.Tags.Is("weather");
		}

		/// <summary>
		/// テスト用に実装
		/// </summary>
		private class MediaFileModelImpl : MediaFileModel {
			public MediaFileModelImpl(string filePath) : base(filePath) {
			}
		}
	}
}
