using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.TestUtilities.TestData;

namespace SandBeige.MediaBox.Tests.Models.Media {
	internal class ImageFileModelTest : MediaFileTest {

		protected override MediaFileModel GetInstance(string filePath) {
			return new ImageFileModel(filePath);
		}

		[Test]
		public void イメージ読み込み() {
			using var ifm = this.GetInstance(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			var args = new List<PropertyChangedEventArgs>();
			ifm.PropertyChanged += (sender, e) => {
				args.Add(e);
			};
			args.Count.Is(0);
			ifm.Image.IsNull();
			ifm.LoadImage();
			args.Select(x => x.PropertyName).Is("Image");
			ifm.Image.IsNotNull();
			ifm.LoadImage();
			args.Select(x => x.PropertyName).Is("Image", "Image");
		}

		[Test]
		public void イメージ読み込み2() {
			using var ifm = this.GetInstance(this.TestFiles.Image1Jpg.FilePath) as ImageFileModel;
			var args = new List<PropertyChangedEventArgs>();
			ifm.PropertyChanged += (sender, e) => {
				args.Add(e);
			};
			args.Count.Is(0);
			ifm.Image.IsNull();
			ifm.LoadImageIfNotLoaded();
			args.Select(x => x.PropertyName).Is("Image");
			ifm.Image.IsNotNull();
			ifm.LoadImageIfNotLoaded();
			args.Select(x => x.PropertyName).Is("Image");
		}

		[Test]
		public void 不正イメージ読み込み() {
			using var ifm = this.GetInstance(this.TestFiles.InvalidJpg.FilePath) as ImageFileModel;
			var args = new List<PropertyChangedEventArgs>();
			ifm.PropertyChanged += (sender, e) => {
				args.Add(e);
			};
			args.Count.Is(0);
			ifm.Image.IsNull();
			ifm.IsInvalid.IsFalse();
			ifm.LoadImageIfNotLoaded();
			args.Count.Is(1);
			args.Select(x => x.PropertyName).Is("IsInvalid");
			ifm.Image.IsNull();
			ifm.IsInvalid.IsTrue();
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
			ifm.ThumbnailFilePath.IsNull();
			ifm.CreateThumbnail();
			ifm.ThumbnailCreated.IsTrue();
			ifm.ThumbnailFilePath.IsNotNull();
		}

		[Test]
		public override void データベース登録と読み込み() {
			var (r, _) = this.Register(this.TestFiles.Image1Jpg);
			r.Rate = 4;
			this.DocumentDb.GetMediaFilesCollection().Update(r);

			var row = this
				.DocumentDb
				.GetMediaFilesCollection()
				.Query()
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

			record.Tags = new string[] { };
			var test = this.TestFiles.Image1Jpg;
			test.Rate = 4;
			record.Check(test);
		}

		[Test]
		public void 特殊ファイル名() {
			var record = new MediaFile();
			using var media = this.GetInstance(this.TestFiles.SpecialFileNameImageJpg.FilePath) as ImageFileModel;
			media.UpdateDataBaseRecord(record);

			media.Check(this.TestFiles.SpecialFileNameImageJpg);
			record.Tags = new string[] { };
			record.Check(this.TestFiles.SpecialFileNameImageJpg);
		}

		[TestCase(TestFileNames.Image1Jpg)]
		[TestCase(TestFileNames.Image2Jpg)]
		[TestCase(TestFileNames.Image3Jpg)]
		[TestCase(TestFileNames.Image4Png)]
		[TestCase(TestFileNames.Image5Bmp)]
		[TestCase(TestFileNames.Image6Gif)]
		[TestCase(TestFileNames.NoExifJpg)]
		[TestCase(TestFileNames.InvalidJpg)]
		public void ファイルパターン(string name) {
			var file = this.TestFiles.Single(x => x.FileName == name);
			var record = new MediaFile();
			using (var media = this.GetInstance(file.FilePath)) {
				media.UpdateDataBaseRecord(record);
			}

			record.Tags = new string[] { };
			record.Check(file);
		}
	}
}
