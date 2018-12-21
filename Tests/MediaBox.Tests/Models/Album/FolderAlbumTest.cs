using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class FolderAlbumTest : TestClassBase {
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

			using (var album1 = Get.Instance<FolderAlbumForTest>(TestDirectories["1"])) {

				await Task.Delay(100);

				album1.Items.Count.Is(7);
				album1.Items.Select(x => x.FilePath.Value).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["1"], "image6.jpg"),
					Path.Combine(TestDirectories["1"], "image8.jpg"),
					Path.Combine(TestDirectories["sub"], "image3.jpg"),
					Path.Combine(TestDirectories["sub"], "image7.jpg")
				);

				// 2回目実行しても同じファイルは追加されない
				album1.CallLoadFileInDirectory(TestDirectories["1"]);
				await Task.Delay(100);
				album1.Items.Count.Is(7);

				// 存在しないフォルダの場合は何も起こらない
				album1.CallLoadFileInDirectory($"{TestDirectories["1"]}____");
				await Task.Delay(100);
				album1.Items.Count.Is(7);
			}
		}

		[Test]
		public void FolderAlbum() {
			using (var album = Get.Instance<FolderAlbumForTest>(TestDirectories["0"])) {
				album.MonitoringDirectories.Count.Is(1);
				album.MonitoringDirectories.Is(
					TestDirectories["0"]);
				album.Title.Value.Is(TestDirectories["0"]);
			}
		}

		[Test]
		public async Task OnAddedItemAsync() {
			using (var album1 = Get.Instance<FolderAlbumForTest>(TestDirectories["1"])) {
				using (var media1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg")))
				using (var media2 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg"))) {
					var thumbDir = Get.Instance<ISettings>().PathSettings.ThumbnailDirectoryPath.Value;
					media1.MediaFileId.IsNull();
					media1.Exif.Value.IsNull();
					media1.Thumbnail.Value.IsNull();
					Directory.GetFiles(thumbDir).Length.Is(0);

					await album1.CallOnAddedItemAsync(media1);
					media1.MediaFileId.IsNull();
					media1.Exif.Value.IsNotNull();
					media1.Thumbnail.Value.IsNotNull();
					Directory.GetFiles(thumbDir).Length.Is(0);
					Assert.AreEqual(35.6517139, media1.Latitude.Value, 0.00001);
					Assert.AreEqual(136.821275, media1.Longitude.Value, 0.00001);
					media1.Orientation.Value.Is(1);
				}
			}
		}

		/// <summary>
		/// protectedメソッドを呼び出すためのテスト用クラス
		/// </summary>
		private class FolderAlbumForTest : FolderAlbum {
			public void CallLoadFileInDirectory(string directoryPath) {
				this.LoadFileInDirectory(directoryPath);
			}

			public async Task CallOnAddedItemAsync(MediaFile mediaFile) {
				await this.OnAddedItemAsync(mediaFile);
			}

			public FolderAlbumForTest(string path) : base(path) {
			}
		}
	}
}
