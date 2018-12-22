using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Media {
	[TestFixture]
	internal class MediaFileTest : TestClassBase {

		[Test]
		public void MediaFile() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				media.FilePath.Value.Is(path);
				media.FileName.Value.Is("image1.jpg");
			}

			path = Path.Combine(TestDirectories["0"], "image2.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				media.FilePath.Value.Is(path);
				media.FileName.Value.Is("image2.jpg");
			}
		}

		[Test]
		public async Task CreateThumbnailAsync() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			var pool = Get.Instance<ThumbnailPool>();
			using (var media = Get.Instance<MediaFile>(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				await media.CreateThumbnailAsync(ThumbnailLocation.Memory);
				pool.Resolve(path).IsNotNull();
				media.Thumbnail.Value.IsNotNull();
				pool.Resolve(path).Is(media.Thumbnail.Value.Image);
			}
			path = Path.Combine(TestDirectories["0"], "image2.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				await media.CreateThumbnailAsync(ThumbnailLocation.Memory);
				pool.Resolve(path).IsNotNull();
				media.Thumbnail.Value.IsNotNull();
				pool.Resolve(path).Is(media.Thumbnail.Value.Image);
			}
			path = Path.Combine(TestDirectories["0"], "image3.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				await media.CreateThumbnailAsync(ThumbnailLocation.File);
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNotNull();
				media.Thumbnail.Value.FilePath.IsNotNull();
				using (var fs = new FileStream(media.Thumbnail.Value.FilePath, FileMode.Open)) {
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.StreamSource = fs;
					bitmap.EndInit();
					bitmap.Freeze();
				}
			}
			path = Path.Combine(TestDirectories["0"], "image4.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNull();
				await media.CreateThumbnailAsync(ThumbnailLocation.File);
				pool.Resolve(path).IsNull();
				media.Thumbnail.Value.IsNotNull();
				media.Thumbnail.Value.FilePath.IsNotNull();
				using (var fs = new FileStream(media.Thumbnail.Value.FilePath, FileMode.Open)) {
					var bitmap = new BitmapImage();
					bitmap.BeginInit();
					bitmap.StreamSource = fs;
					bitmap.EndInit();
					bitmap.Freeze();
				}
			}
		}

		[Test]
		public async Task LoadExifIfNotLoadedAsync() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				media.Exif.Value.IsNull();
				await media.LoadExifIfNotLoadedAsync();
				media.Exif.Value.IsNotNull();
				await media.LoadExifIfNotLoadedAsync();
				media.Exif.Value.IsNotNull();
			}
		}

		[Test]
		public async Task LoadExifAsync() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				media.Exif.Value.IsNull();
				await media.LoadExifAsync();
				media.Exif.Value.IsNotNull();
				Assert.AreEqual(35.6517139, media.Latitude.Value, 0.00001);
				Assert.AreEqual(136.821275, media.Longitude.Value, 0.00001);
				media.Orientation.Value.Is(1);
			}
		}

		[Test]
		public async Task AddTagRemoveTag() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			var db = Get.Instance<MediaBoxDbContext>();
			using (var media = Get.Instance<MediaFile>(path)) {
				media.Tags.Count.Is(0);
				db.MediaFileTags.Count().Is(0);
				db.Tags.Count().Is(0);

				// DB登録されていないmedia
				media.AddTag("tag");
				media.Tags.Count.Is(0);
				db.MediaFileTags.Count().Is(0);
				db.Tags.Count().Is(0);
			}

			using (var album = Get.Instance<RegisteredAlbumForTest>()) {
				album.Create();
				using (var media1 = Get.Instance<MediaFile>(path))
				using (var media2 = Get.Instance<MediaFile>(path)) {
					media2.Tags.Count.Is(0);
					db.MediaFileTags.Count().Is(0);
					db.Tags.Count().Is(0);

					await album.CallOnAddedItemAsync(media1);
					await album.CallOnAddedItemAsync(media2);
					media2.AddTag("tag");
					media2.Tags.Count.Is(1);
					media2.Tags.Single().Is("tag");
					db.MediaFileTags.Count().Is(1);
					db.Tags.Count().Is(1);

					var tag = await db.Tags.SingleAsync();
					var mediaFileTag = await db.MediaFileTags.SingleAsync();
					tag.TagName.Is("tag");
					tag.TagId.Is(1);
					mediaFileTag.TagId.Is(1);
					mediaFileTag.MediaFileId.Is(2);

					// 追加済みのタグは無視される
					media2.AddTag("tag");

					media2.Tags.Count.Is(1);
					db.MediaFileTags.Count().Is(1);
					db.Tags.Count().Is(1);

					media2.AddTag("tag2");
					media1.AddTag("tag2");
					media1.AddTag("tag");

					media2.Tags.Count.Is(2);
					db.MediaFileTags.Count().Is(4);
					db.Tags.Count().Is(4);

					media2.RemoveTag("tag2");

					media2.Tags.Count.Is(1);
					//タグとメディアのリレーションは消える
					db.MediaFileTags.Count().Is(3);
					// 登録したタグは消えない
					db.Tags.Count().Is(4);
					db.MediaFileTags
						.Include(x => x.Tag)
						.ToList()
						.Select(x => $"[{x.MediaFileId}],{x.Tag.TagName}")
						.Is(
							"[2],tag",
							"[1],tag2",
							"[1],tag");

					// 存在しないタグは何も起こらない
					media2.RemoveTag("tag2");
					media2.Tags.Count.Is(1);
					db.MediaFileTags.Count().Is(3);

					// もしIDがnull(=DB登録されていない状態)だったら何も起こらない
					media2.MediaFileId = null;
					media2.RemoveTag("tag");
					media2.Tags.Count.Is(1);
					db.MediaFileTags.Count().Is(3);
				}
			}
		}

		[Test]
		public async Task SetGps() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			var db = Get.Instance<MediaBoxDbContext>();
			using (var media = Get.Instance<MediaFile>(path)) {
				media.Latitude.Value.IsNull();
				media.Longitude.Value.IsNull();

				// DB登録されていないmedia
				media.SetGps(50.15, 40.36);
				media.Latitude.Value.IsNull();
				media.Longitude.Value.IsNull();
			}

			using (var album = Get.Instance<RegisteredAlbumForTest>()) {
				album.Create();
				using (var media1 = Get.Instance<MediaFile>(path))
				using (var media2 = Get.Instance<MediaFile>(path)) {
					await album.CallOnAddedItemAsync(media1);
					await album.CallOnAddedItemAsync(media2);

					var dbMedia1 = db.MediaFiles.Single(x => x.MediaFileId == media1.MediaFileId);
					var dbMedia2 = db.MediaFiles.Single(x => x.MediaFileId == media2.MediaFileId);

					media2.SetGps(50.15, 40.36);
					media1.SetGps(60.173, 62.44);

					dbMedia2.Latitude.Is(50.15);
					media2.Latitude.Value.Is(50.15);
					dbMedia2.Longitude.Is(40.36);
					media2.Longitude.Value.Is(40.36);

					dbMedia1.Latitude.Is(60.173);
					media1.Latitude.Value.Is(60.173);
					dbMedia1.Longitude.Is(62.44);
					media1.Longitude.Value.Is(62.44);
				}
			}
		}

		[Test]
		public async Task LoadImageUnloadImage() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media1 = Get.Instance<MediaFile>(path))
			using (var media2 = Get.Instance<MediaFile>(path)) {
				media1.Image.Value.IsNull();
				media1.Orientation.Value = 1;
				await media1.LoadImageAsync();
				media1.Image.Value.IsNotNull();
				((BitmapSource)media1.Image.Value).PixelHeight.Is(3024);
				((BitmapSource)media1.Image.Value).PixelWidth.Is(4032);

				media2.Image.Value.IsNull();
				media2.Orientation.Value = 6;
				await media2.LoadImageAsync();
				media2.Image.Value.IsNotNull();
				((BitmapSource)media2.Image.Value).PixelHeight.Is(4032);
				((BitmapSource)media2.Image.Value).PixelWidth.Is(3024);

				media1.UnloadImage();
				media1.Image.Value.IsNull();
				media2.UnloadImage();
				media2.Image.Value.IsNull();
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
