using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
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
				db.Albums.Count().Is(0);
				album1.AlbumId.Value.Is(0);

				// 作成するとDB登録される
				album1.Create();
				db.Albums.Count().Is(1);
				album1.AlbumId.Value.Is(1);

				// 作成するとDB登録される
				album2.Create();
				db.Albums.Count().Is(2);
				album2.AlbumId.Value.Is(2);
			}
		}

		[Test]
		public void LoadFromDataBase() {
			var db = Get.Instance<MediaBoxDbContext>();
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>())
			using (var album3 = Get.Instance<RegisteredAlbum>()) {
				db.Albums.Count().Is(0);

				// 存在しないアルバム
				Assert.Catch<InvalidOperationException>(() => album1.LoadFromDataBase(1));

				album1.Items.Count.Is(0);
				album1.Count.Value.Is(0);
				album1.Directories.Count.Is(0);
				album1.Title.Value.IsNull();
				album1.AlbumPath.Value.Is("");

				// アルバム2と3を作って登録
				album2.Create();
				album2.Title.Value = "album2";
				album2.AlbumPath.Value = "/iphone/path";
				album2.Directories.Add(TestDirectories["2"]);
				album2.Directories.Add(TestDirectories["4"]);
				album2.Directories.Add(TestDirectories["6"]);
				album2.AddFiles(new[]{
					this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg")),
					this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg")),
					this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"))
				});
				album2.ReflectToDataBase();
				db.Albums.Count().Is(1);
				album2.AlbumId.Value.Is(1);

				album3.Create();
				album3.Title.Value = "album3";
				album3.AlbumPath.Value = "/android/pen";
				album3.Directories.Add(TestDirectories["3"]);
				album3.Directories.Add(TestDirectories["5"]);
				album3.AddFiles(new[]{
					this.MediaFactory.Create(Path.Combine(TestDataDir, "image4.jpg")),
					this.MediaFactory.Create(Path.Combine(TestDataDir, "image5.jpg"))
				});
				album3.ReflectToDataBase();
				db.Albums.Count().Is(2);
				album3.AlbumId.Value.Is(2);

				// アルバム1にアルバム2のデータを読み込む
				album1.LoadFromDataBase(1);

				album1.AlbumId.Value.Is(1);
				album1.Title.Value.Is("album2");
				album1.AlbumPath.Value.Is("/iphone/path");
				album1.Items.Count.Is(3);
				album1.Directories.Is(
					TestDirectories["2"],
					TestDirectories["4"],
					TestDirectories["6"]);
				album1.Items.Select(x => x.FileName).OrderBy(x => x).Is(
					"image1.jpg",
					"image2.jpg",
					"image3.jpg");
			}
		}

		[Test]
		public void AddRemoveFiles() {
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>())
			using (var album3 = Get.Instance<RegisteredAlbum>()) {
				var db = Get.Instance<MediaBoxDbContext>();
				var image1 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
				var image2 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
				var image3 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
				var image4 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image4.jpg"));
				var image5 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image5.jpg"));
				// アルバム3つ作成
				album1.Create();
				album2.Create();
				album3.Create();

				// 初期値
				album1.Items.Count.Is(0);
				image1.Thumbnail.IsNull();
				db.MediaFiles.Count().Is(0);
				db.AlbumMediaFiles.Count().Is(0);

				// アルバム1に1,2,5を追加
				album1.AddFiles(new[]{
					image1,
					image2,
					image5
				});
				// アルバム2に3を追加
				album2.AddFiles(new[] { image3 });
				// アルバム3に4,5を追加
				album3.AddFiles(new[]{
					image4,
					image5
				});

				// アルバムに追加されるとサムネイルとExifが読み込まれる
				image1.Thumbnail.IsNotNull();
				// データベースに登録されている
				// アルバム1,3に登録されたimage5は重複登録されない
				db.MediaFiles.Count().Is(5);
				db.AlbumMediaFiles.Count().Is(6);

				album1.Items.Count.Is(3);
				album1.Items.OrderBy(x => x.FileName).Is(image1, image2, image5);
				album2.Items.Count.Is(1);
				album2.Items.Is(image3);
				album3.Items.Count.Is(2);
				album3.Items.OrderBy(x => x.FileName).Is(
					image4,
					image5);

				album3.RemoveFiles(new[] { image5 });
				album3.Items.Is(image4);

				album1.RemoveFiles(new[] { image1, image2 });

				album1.Items.Is(image5);

				// 一度登録されたイメージは削除されない
				db.MediaFiles.Count().Is(5);
				// アルバムとのリレーションは解除される
				db.AlbumMediaFiles.Count().Is(3);
			}
		}

		[Test]
		public void LoadFileInDirectoryAsync() {
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.ImageExtensions.Clear();
			settings.GeneralSettings.ImageExtensions.Add(".jpg");

			FileUtility.Copy(
				TestDataDir,
				TestDirectories["1"],
				new[] { "image1.jpg", "image2.jpg", "image4.jpg", "image6.jpg", "image8.jpg", "image9.png" });


			FileUtility.Copy(
				TestDataDir,
				TestDirectories["sub"],
				new[] { "image3.jpg", "image7.jpg" });


			FileUtility.Copy(
				TestDataDir,
				TestDirectories["2"],
				new[] { "image5.jpg" });

			using (var album1 = Get.Instance<RegisteredAlbum>()) {

				album1.Create();
				album1.Directories.Add(TestDirectories["1"]);
				album1.Items.Count.Is(7);
				album1.Items.Select(x => x.FilePath).OrderBy(x => x).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["1"], "image6.jpg"),
					Path.Combine(TestDirectories["1"], "image8.jpg"),
					Path.Combine(TestDirectories["sub"], "image3.jpg"),
					Path.Combine(TestDirectories["sub"], "image7.jpg"));

				album1.Directories.Add(TestDirectories["2"]);

				album1.Items.Count.Is(8);
				album1.Items.Select(x => x.FilePath).OrderBy(x => x).Is(
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
	}
}
