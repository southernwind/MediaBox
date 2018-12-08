﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SandBeige.MediaBox.TestUtilities;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class RegisteredAlbumTest : TestClassBase {

		[Test]
		public void Create() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>()) {
				// インスタンス生成時点では何も起きない
				Assert.AreEqual(0, db.Albums.Count());
				Assert.AreEqual(0, album1.AlbumId);

				// 作成するとDB登録される
				album1.Create();
				Assert.AreEqual(1, db.Albums.Count());
				Assert.AreEqual(1, album1.AlbumId);

				// 作成するとDB登録される
				album2.Create();
				Assert.AreEqual(2, db.Albums.Count());
				Assert.AreEqual(2, album2.AlbumId);

				// 作成後、再作成しようとしたり、DB読み込みしようとしたりすると例外
				Assert.Catch<InvalidOperationException>(album1.Create);
				Assert.Catch<InvalidOperationException>(() => album2.LoadFromDataBase(album2.AlbumId));
			}
		}

		[Test]
		public async Task LoadFromDataBase() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbumForTest>())
			using (var album2 = Get.Instance<RegisteredAlbumForTest>())
			using (var album3 = Get.Instance<RegisteredAlbumForTest>()) {
				Assert.AreEqual(0, db.Albums.Count());

				// データベースにないと当然読み込めない
				Assert.Catch<InvalidOperationException>(() => album1.LoadFromDataBase(1));
				Assert.AreEqual(0, album1.Items.Count);
				Assert.AreEqual(0, album1.Count.Value);
				Assert.AreEqual(0, album1.MonitoringDirectories.Count);
				Assert.AreEqual(null, album1.Title.Value);

				// アルバム2と3を作って登録
				album2.Create();
				album2.Title.Value = "album2";
				album2.MonitoringDirectories.Add(TestDirectories["2"]);
				album2.MonitoringDirectories.Add(TestDirectories["4"]);
				album2.MonitoringDirectories.Add(TestDirectories["6"]);
				await album2.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg")));
				await album2.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg")));
				await album2.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image3.jpg")));
				Assert.AreEqual(1, db.Albums.Count());
				album3.Create();
				album3.Title.Value = "album3";
				album3.MonitoringDirectories.Add(TestDirectories["3"]);
				album3.MonitoringDirectories.Add(TestDirectories["5"]);
				await album3.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image4.jpg")));
				await album3.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image5.jpg")));
				Assert.AreEqual(2, db.Albums.Count());

				// アルバム2のデータを読み込む
				album1.LoadFromDataBase(1);

				Assert.AreEqual(1, album1.AlbumId);
				Assert.AreEqual("album2", album1.Title.Value);
				Assert.AreEqual(3, album1.Items.Count);
				CollectionAssert.AreEquivalent(
					new[] {
						TestDirectories["2"],
						TestDirectories["4"],
						TestDirectories["6"]
					}, album1.MonitoringDirectories);
				CollectionAssert.AreEquivalent(
					new[] {
						"image1.jpg",
						"image2.jpg",
						"image3.jpg"
					}, album1.Items.Select(x => x.FileName.Value));

				// 2回目読み込みは例外
				Assert.Catch<InvalidOperationException>(() => album1.LoadFromDataBase(1));
				Assert.Catch<InvalidOperationException>(() => album2.LoadFromDataBase(1));
			}
		}

		[Test]
		public void AddFile() {
			using (var album1 = Get.Instance<RegisteredAlbumForTest>())
			using (var album2 = Get.Instance<RegisteredAlbumForTest>())
			using (var album3 = Get.Instance<RegisteredAlbumForTest>()) {

				Assert.Catch<InvalidOperationException>(() => {
					album1.AddFile(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg")));
				});
				album1.Create();
				album2.Create();
				album3.Create();

				Assert.Catch<ArgumentNullException>(() => {
					album1.AddFile(null);
				});

				Assert.AreEqual(0, album1.Items.Count);

				album1.AddFile(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg")));
				album1.AddFile(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg")));
				album1.AddFile(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image5.jpg")));

				album2.AddFile(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image3.jpg")));
				album3.AddFile(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image4.jpg")));
				album3.AddFile(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image5.jpg")));
				Assert.AreEqual(3, album1.Items.Count);
				CollectionAssert.AreEqual(
					new[] {
						"image1.jpg",
						"image2.jpg",
						"image5.jpg"
					}, album1.Items.Select(x => x.FileName.Value));
				Assert.AreEqual(1, album2.Items.Count);
				CollectionAssert.AreEqual(
					new[] {
						"image3.jpg"
					}, album2.Items.Select(x => x.FileName.Value));
				Assert.AreEqual(2, album3.Items.Count);
				CollectionAssert.AreEqual(
					new[] {
						"image4.jpg",
						"image5.jpg"
					}, album3.Items.Select(x => x.FileName.Value));
			}
		}

		[Test]
		public void LoadFileInDirectory() {
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.TargetExtensions.Value = new[] { ".jpg" };

			FileUtility.Copy(
				TestDirectories["0"],
				TestDirectories["1"],
				new[] { "image1.jpg", "image2.jpg", "image4.jpg", "image6.jpg", "image8.jpg", "image9.png" });


			FileUtility.Copy(
				TestDirectories["0"],
				TestDirectories["sub"],
				new[] { "image3.jpg", "image7.jpg" });


			FileUtility.Copy(
				TestDirectories["0"],
				TestDirectories["2"],
				new[] { "image5.jpg" });

			using (var album1 = Get.Instance<RegisteredAlbumForTest>()) {
				album1.Create();
				album1.CallLoadFileInDirectory(TestDirectories["1"]);

				Assert.AreEqual(7, album1.Items.Count);
				CollectionAssert.AreEqual(new[] {
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["1"], "image6.jpg"),
					Path.Combine(TestDirectories["1"], "image8.jpg"),
					Path.Combine(TestDirectories["sub"], "image3.jpg"),
					Path.Combine(TestDirectories["sub"], "image7.jpg")
				}, album1.Items.Select(x => x.FilePath.Value));

				album1.CallLoadFileInDirectory(TestDirectories["2"]);

				Assert.AreEqual(8, album1.Items.Count);
				CollectionAssert.AreEqual(new[] {
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["1"], "image6.jpg"),
					Path.Combine(TestDirectories["1"], "image8.jpg"),
					Path.Combine(TestDirectories["sub"], "image3.jpg"),
					Path.Combine(TestDirectories["sub"], "image7.jpg"),
					Path.Combine(TestDirectories["2"], "image5.jpg")
				}, album1.Items.Select(x => x.FilePath.Value));
			}
		}

		[Test]
		public async Task OnAddedItemAsync() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album0 = Get.Instance<RegisteredAlbumForTest>())
			using (var album00 = Get.Instance<RegisteredAlbumForTest>())
			using (var album000 = Get.Instance<RegisteredAlbumForTest>())
			using (var album1 = Get.Instance<RegisteredAlbumForTest>()) {

				album0.Create();
				album00.Create();
				album000.Create();

				album1.Create();
				using (var media1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg")))
				using (var media2 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg"))) {
					var thumbDir = Get.Instance<ISettings>().PathSettings.ThumbnailDirectoryPath.Value;
					Assert.IsNull(media1.MediaFileId);
					Assert.IsNull(media1.Exif.Value);
					Assert.IsNull(media1.Thumbnail.Value);
					Assert.AreEqual(0, Directory.GetFiles(thumbDir).Length);
					Assert.AreEqual(0, db.AlbumMediaFiles.Count());
					Assert.AreEqual(0, db.MediaFiles.Count());

					await album1.CallOnAddedItemAsync(media1);
					Assert.AreEqual(1, media1.MediaFileId);
					Assert.IsNotNull(media1.Exif.Value);
					Assert.IsNotNull(media1.Thumbnail.Value);
					Assert.AreEqual(1, Directory.GetFiles(thumbDir).Length);
					Assert.AreEqual(1, db.AlbumMediaFiles.Count());
					Assert.AreEqual(1, db.MediaFiles.Count());

					var amf = await db.AlbumMediaFiles.Include(x => x.MediaFile).FirstAsync();
					Assert.AreEqual(4, amf.AlbumId);
					Assert.AreEqual(TestDirectories["0"], amf.MediaFile.DirectoryPath);
					Assert.AreEqual("image1.jpg", amf.MediaFile.FileName);
					Assert.AreEqual(Path.GetFileName(Directory.GetFiles(thumbDir)[0]), amf.MediaFile.ThumbnailFileName);
					Assert.AreEqual(35.6517139, amf.MediaFile.Latitude, 0.00001);
					Assert.AreEqual(136.821275, amf.MediaFile.Longitude, 0.00001);
					Assert.AreEqual(1, amf.MediaFile.Orientation);
				}
			}
		}
		
		[Test]
		public void Title() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbumForTest>())
			using (var album2 = Get.Instance<RegisteredAlbumForTest>()) {
				album1.Title.Value = "Sweet";

				Assert.IsNull(db.Albums.SingleOrDefault(x => x.AlbumId == 1));

				album1.Create();
				album2.Create();

				var album1Row = db.Albums.Single(x => x.AlbumId == 1);
				var album2Row = db.Albums.Single(x => x.AlbumId == 2);

				album1.Title.Value = "";

				Assert.AreEqual("", album1Row.Title);
				Assert.IsNull(album2Row.Title);

				album2.Title.Value = "AppleBanana";

				album1Row = db.Albums.Single(x => x.AlbumId == 1);
				album2Row = db.Albums.Single(x => x.AlbumId == 2);

				Assert.AreEqual("", album1Row.Title);
				Assert.AreEqual("AppleBanana", album2Row.Title);
			}
		}

		[Test]
		public void MonitoringDirectories() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbumForTest>())
			using (var album2 = Get.Instance<RegisteredAlbumForTest>()) {

				album1.MonitoringDirectories.Add(TestDirectories["0"]);

				Assert.IsNull(db.Albums.SingleOrDefault(x => x.AlbumId == 1));

				album1.Create();
				album2.Create();

				var album1Row = db.Albums.Include(x => x.AlbumDirectories).Single(x => x.AlbumId == 1);
				var album2Row = db.Albums.Include(x => x.AlbumDirectories).Single(x => x.AlbumId == 2);

				// 追加1
				album1.MonitoringDirectories.Add(TestDirectories["2"]);

				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["2"]
				}, album1Row.AlbumDirectories.Select(x => x.Directory));
				CollectionAssert.IsEmpty(album2Row.AlbumDirectories);

				// 追加2
				album2.MonitoringDirectories.Add(TestDirectories["4"]);
				album2.MonitoringDirectories.Add(TestDirectories["5"]);
				album2.MonitoringDirectories.Add(TestDirectories["6"]);
				album2.MonitoringDirectories.Add(TestDirectories["3"]);
				album2.MonitoringDirectories.Add(TestDirectories["2"]);

				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["2"]
				}, album1Row.AlbumDirectories.Select(x => x.Directory));
				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["2"],
					TestDirectories["3"],
					TestDirectories["4"],
					TestDirectories["5"],
					TestDirectories["6"]
				}, album2Row.AlbumDirectories.Select(x => x.Directory));

				// 削除
				album2.MonitoringDirectories.Remove(TestDirectories["3"]);
				album2.MonitoringDirectories.Remove(TestDirectories["2"]);

				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["2"]
				}, album1Row.AlbumDirectories.Select(x => x.Directory));
				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["4"],
					TestDirectories["5"],
					TestDirectories["6"]
				}, album2Row.AlbumDirectories.Select(x => x.Directory));
			}
		}

		/// <summary>
		/// protectedメソッドを呼び出すためのテスト用クラス
		/// </summary>
		private class RegisteredAlbumForTest : RegisteredAlbum {
			public void CallLoadFileInDirectory(string directoryPath) {
				this.LoadFileInDirectory(directoryPath);
			}

			public async Task CallOnAddedItemAsync(MediaFile mediaFile) {
				await this.OnAddedItemAsync(mediaFile);
			}
		}
	}
}
