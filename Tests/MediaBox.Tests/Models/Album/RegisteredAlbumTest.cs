using System;
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
using Reactive.Bindings;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class RegisteredAlbumTest : TestClassBase {

		[Test]
		public void Create() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>()) {
				// インスタンス生成時点では何も起きない
				db.Albums.Count().Is(0);
				album1.AlbumId.Is(0);

				// 作成するとDB登録される
				album1.Create();
				db.Albums.Count().Is(1);
				album1.AlbumId.Is(1);

				// 作成するとDB登録される
				album2.Create();
				db.Albums.Count().Is(2);
				album2.AlbumId.Is(2);

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
				db.Albums.Count().Is(0);

				// データベースにないと当然読み込めない
				Assert.Catch<InvalidOperationException>(() => album1.LoadFromDataBase(1));
				album1.Items.Count.Is(0);
				album1.Count.Value.Is(0);
				album1.MonitoringDirectories.Count.Is(0);
				album1.Title.Value.IsNull();

				// アルバム2と3を作って登録
				album2.Create();
				album2.Title.Value = "album2";
				album2.MonitoringDirectories.Add(TestDirectories["2"]);
				album2.MonitoringDirectories.Add(TestDirectories["4"]);
				album2.MonitoringDirectories.Add(TestDirectories["6"]);
				await album2.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg")));
				await album2.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg")));
				await album2.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image3.jpg")));
				db.Albums.Count().Is(1);
				album3.Create();
				album3.Title.Value = "album3";
				album3.MonitoringDirectories.Add(TestDirectories["3"]);
				album3.MonitoringDirectories.Add(TestDirectories["5"]);
				await album3.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image4.jpg")));
				await album3.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image5.jpg")));
				db.Albums.Count().Is(2);

				// アルバム2のデータを読み込む
				album1.LoadFromDataBase(1);

				album1.AlbumId.Is(1);
				album1.Title.Value.Is("album2");
				album1.Items.Count.Is(3);
				album1.MonitoringDirectories.Is(
					TestDirectories["2"],
					TestDirectories["4"],
					TestDirectories["6"]);
				album1.Items.Select(x => x.FileName.Value).Is(
					"image1.jpg",
					"image2.jpg",
					"image3.jpg");

				// 2回目読み込みは例外
				Assert.Catch<InvalidOperationException>(() => album1.LoadFromDataBase(1));
				Assert.Catch<InvalidOperationException>(() => album2.LoadFromDataBase(1));
			}
		}

		[Test]
		public async Task AddRemoveFiles() {
			using (var album1 = Get.Instance<RegisteredAlbumForTest>())
			using (var album2 = Get.Instance<RegisteredAlbumForTest>())
			using (var album3 = Get.Instance<RegisteredAlbumForTest>()) {

				var image1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				var image2 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg"));
				var image3 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image3.jpg"));
				var image4 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image4.jpg"));
				var image5 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image5.jpg"));
				Assert.Catch<InvalidOperationException>(() => {
					album1.AddFiles(new []{
						image1
					});
				});
				album1.Create();
				album2.Create();
				album3.Create();

				Assert.Catch<ArgumentNullException>(() => {
					album1.AddFiles(null);
				});

				album1.Items.Count.Is(0);

				album1.AddFiles(new []{
					image1,
					image2,
					image5
				});

				album2.AddFiles(new[] { image3 });
				album3.AddFiles(new []{
					image4,
					image5
				});
				await Task.Delay(100);
				album1.Items.Count.Is(3);
				album1.Items.Is(image1, image2, image5);
				await Task.Delay(100);
				album2.Items.Count.Is(1);
				album2.Items.Is(image3);
				await Task.Delay(100);
				album3.Items.Count.Is(2);
				album3.Items.Is(
					image4,
					image5);

				album3.RemoveFiles(new[] { image5 });
				album3.Items.Is(image4);

				album1.RemoveFiles(new[] { image1, image2 });

				album1.Items.Is(image5);
			}
		}

		[Test]
		public async Task LoadFileInDirectory() {
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
				await Task.Delay(100);
				album1.Items.Count.Is(7);
				album1.Items.Select(x => x.FilePath.Value).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["1"], "image6.jpg"),
					Path.Combine(TestDirectories["1"], "image8.jpg"),
					Path.Combine(TestDirectories["sub"], "image3.jpg"),
					Path.Combine(TestDirectories["sub"], "image7.jpg"));

				album1.CallLoadFileInDirectory(TestDirectories["2"]);
				await Task.Delay(100);

				album1.Items.Count.Is(8);
				album1.Items.Select(x => x.FilePath.Value).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["1"], "image6.jpg"),
					Path.Combine(TestDirectories["1"], "image8.jpg"),
					Path.Combine(TestDirectories["sub"], "image3.jpg"),
					Path.Combine(TestDirectories["sub"], "image7.jpg"),
					Path.Combine(TestDirectories["2"], "image5.jpg")
				);
			}
		}

		[Test]
		public async Task OnAddedRemovedItemAsync() {
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
					media1.MediaFileId.IsNull();
					media1.Exif.Value.IsNull();
					media1.Thumbnail.Value.IsNull();
					Directory.GetFiles(thumbDir).Length.Is(0);
					db.AlbumMediaFiles.Count().Is(0);
					db.MediaFiles.Count().Is(0);

					await album1.CallOnAddedItemAsync(media1);
					media1.MediaFileId.Is(1);
					media1.Exif.Value.IsNotNull();
					media1.Thumbnail.Value.IsNotNull();
					Directory.GetFiles(thumbDir).Length.Is(1);
					db.AlbumMediaFiles.Count().Is(1);
					db.MediaFiles.Count().Is(1);

					var amf = await db.AlbumMediaFiles.Include(x => x.MediaFile).FirstAsync();
					db.AlbumMediaFiles.Count().Is(1);
					db.MediaFiles.Count().Is(1);
					amf.AlbumId.Is(4);
					amf.MediaFile.DirectoryPath.Is(TestDirectories["0"]);
					amf.MediaFile.FileName.Is("image1.jpg");
					amf.MediaFile.ThumbnailFileName.Is(Path.GetFileName(Directory.GetFiles(thumbDir)[0]));
					Assert.AreEqual(35.6517139, amf.MediaFile.Latitude, 0.00001);
					Assert.AreEqual(136.821275, amf.MediaFile.Longitude, 0.00001);
					amf.MediaFile.Orientation.Is(1);

					// Remove
					db.Albums.Count().Is(4);
					await album1.CallOnAddedItemAsync(media2);
					db.AlbumMediaFiles.Count().Is(2);
					db.MediaFiles.Count().Is(2);
					await album1.CallOnRemovedItemAsync(media1);
					db.AlbumMediaFiles.Count().Is(1);
					db.MediaFiles.Count().Is(2);
					(await db.AlbumMediaFiles.FirstAsync()).MediaFile.MediaFileId.Is((long)media2.MediaFileId);
					await album1.CallOnRemovedItemAsync(media2);
					db.AlbumMediaFiles.Count().Is(0);
					db.MediaFiles.Count().Is(2);
					db.Albums.Count().Is(4);
				}
			}
		}
		
		[Test]
		public void Title() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbumForTest>())
			using (var album2 = Get.Instance<RegisteredAlbumForTest>()) {
				album1.Title.Value = "Sweet";

				db.Albums.SingleOrDefault(x => x.AlbumId == 1).IsNull();

				album1.Create();
				album2.Create();

				var album1Row = db.Albums.Single(x => x.AlbumId == 1);
				var album2Row = db.Albums.Single(x => x.AlbumId == 2);

				album1.Title.Value = "";

				album1Row.Title.Is("");
				album2Row.Title.IsNull();

				album2.Title.Value = "AppleBanana";

				album1Row = db.Albums.Single(x => x.AlbumId == 1);
				album2Row = db.Albums.Single(x => x.AlbumId == 2);

				album1Row.Title.Is("");
				album2Row.Title.Is("AppleBanana");
			}
		}

		[Test]
		public void MonitoringDirectories() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbumForTest>())
			using (var album2 = Get.Instance<RegisteredAlbumForTest>()) {

				album1.MonitoringDirectories.Add(TestDirectories["0"]);

				db.Albums.SingleOrDefault(x => x.AlbumId == 1).IsNull();

				album1.Create();
				album2.Create();

				var album1Row = db.Albums.Include(x => x.AlbumDirectories).Single(x => x.AlbumId == 1);
				var album2Row = db.Albums.Include(x => x.AlbumDirectories).Single(x => x.AlbumId == 2);

				// 追加1
				album1.MonitoringDirectories.Add(TestDirectories["2"]);

				album1Row.AlbumDirectories.Select(x => x.Directory).Is(TestDirectories["2"]);
				album2Row.AlbumDirectories.Is();

				// 追加2
				album2.MonitoringDirectories.Add(TestDirectories["4"]);
				album2.MonitoringDirectories.Add(TestDirectories["5"]);
				album2.MonitoringDirectories.Add(TestDirectories["6"]);
				album2.MonitoringDirectories.Add(TestDirectories["3"]);
				album2.MonitoringDirectories.Add(TestDirectories["2"]);

				album1Row.AlbumDirectories.Select(x => x.Directory).Is(TestDirectories["2"]);
				album2Row.AlbumDirectories.Select(x => x.Directory).OrderBy(x => x).Is(
					TestDirectories["2"],
					TestDirectories["3"],
					TestDirectories["4"],
					TestDirectories["5"],
					TestDirectories["6"]
				);

				// 削除
				album2.MonitoringDirectories.Remove(TestDirectories["3"]);
				album2.MonitoringDirectories.Remove(TestDirectories["2"]);

				album1Row.AlbumDirectories.Select(x => x.Directory).Is(TestDirectories["2"]);
				album2Row.AlbumDirectories.Select(x => x.Directory).OrderBy(x => x).Is(
					TestDirectories["4"],
					TestDirectories["5"],
					TestDirectories["6"]);
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

			public async Task CallOnRemovedItemAsync(MediaFile mediaFile) {
				await this.OnRemovedItemAsync(mediaFile);
			}
		}
	}
}
