using System;
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
	internal class MediaFileTest :TestClassBase {

		[Test]
		public void MediaFile() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				Assert.AreEqual(path, media.FilePath.Value);
				Assert.AreEqual("image1.jpg",media.FileName.Value);
			}

			path = Path.Combine(TestDirectories["0"], "image2.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				Assert.AreEqual(path, media.FilePath.Value);
				Assert.AreEqual("image2.jpg", media.FileName.Value);
			}
		}

		[Test]
		public async Task CreateThumbnailAsync() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			var pool = Get.Instance<ThumbnailPool>();
			using (var media = Get.Instance<MediaFile>(path)) {
				Assert.IsNull(pool.Resolve(path));
				Assert.IsNull(media.Thumbnail.Value);
				await media.CreateThumbnailAsync(ThumbnailLocation.Memory);
				Assert.IsNotNull(pool.Resolve(path));
				Assert.IsNotNull(media.Thumbnail.Value);
				Assert.AreEqual(media.Thumbnail.Value.Image, pool.Resolve(path));
			}
			path = Path.Combine(TestDirectories["0"], "image2.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				Assert.IsNull(pool.Resolve(path));
				Assert.IsNull(media.Thumbnail.Value);
				await media.CreateThumbnailAsync(ThumbnailLocation.Memory);
				Assert.IsNotNull(pool.Resolve(path));
				Assert.IsNotNull(media.Thumbnail.Value);
				Assert.AreEqual(media.Thumbnail.Value.Image, pool.Resolve(path));
			}
			path = Path.Combine(TestDirectories["0"], "image3.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				Assert.IsNull(pool.Resolve(path));
				Assert.IsNull(media.Thumbnail.Value);
				await media.CreateThumbnailAsync(ThumbnailLocation.File);
				Assert.IsNull(pool.Resolve(path));
				Assert.IsNotNull(media.Thumbnail.Value);
				Assert.IsNotNull(media.Thumbnail.Value.FilePath);
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
				Assert.IsNull(pool.Resolve(path));
				Assert.IsNull(media.Thumbnail.Value);
				await media.CreateThumbnailAsync(ThumbnailLocation.File);
				Assert.IsNull(pool.Resolve(path));
				Assert.IsNotNull(media.Thumbnail.Value);
				Assert.IsNotNull(media.Thumbnail.Value.FilePath);
				using (var fs = new FileStream(media.Thumbnail.Value.FilePath,FileMode.Open)) {
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
				Assert.IsNull(media.Exif.Value);
				await media.LoadExifIfNotLoadedAsync();
				Assert.IsNotNull(media.Exif.Value);
				await media.LoadExifIfNotLoadedAsync();
				Assert.IsNotNull(media.Exif.Value);
			}
		}

		[Test]
		public async Task LoadExifAsync() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			using (var media = Get.Instance<MediaFile>(path)) {
				Assert.IsNull(media.Exif.Value);
				await media.LoadExifAsync();
				Assert.IsNotNull(media.Exif.Value);
				Assert.AreEqual(35.6517139, media.Latitude.Value, 0.00001);
				Assert.AreEqual(136.821275, media.Longitude.Value, 0.00001);
				Assert.AreEqual(1, media.Orientation.Value);
			}
		}

		[Test]
		public async Task AddTagRemoveTag() {
			var path = Path.Combine(TestDirectories["0"], "image1.jpg");
			var db = Get.Instance<MediaBoxDbContext>();
			using (var media = Get.Instance<MediaFile>(path)) {
				Assert.AreEqual(0, media.Tags.Count);
				Assert.AreEqual(0, db.MediaFileTags.Count());
				Assert.AreEqual(0, db.Tags.Count());

				// DB登録されていないmedia
				media.AddTag("tag");
				Assert.AreEqual(0, media.Tags.Count);
				Assert.AreEqual(0, db.MediaFileTags.Count());
				Assert.AreEqual(0, db.Tags.Count());
			}

			using (var album = Get.Instance<RegisteredAlbumForTest>()) {
				album.Create();
				using (var media1 = Get.Instance<MediaFile>(path))
				using (var media2 = Get.Instance<MediaFile>(path)) {
					Assert.AreEqual(0, media2.Tags.Count);
					Assert.AreEqual(0, db.MediaFileTags.Count());
					Assert.AreEqual(0, db.Tags.Count());

					await album.CallOnAddedItemAsync(media1);
					await album.CallOnAddedItemAsync(media2);
					media2.AddTag("tag");
					Assert.AreEqual(1, media2.Tags.Count);
					Assert.AreEqual("tag", media2.Tags.Single());
					Assert.AreEqual(1, db.MediaFileTags.Count());
					Assert.AreEqual(1, db.Tags.Count());

					var tag = await db.Tags.SingleAsync();
					var mediaFileTag = await db.MediaFileTags.SingleAsync();
					Assert.AreEqual("tag", tag.TagName);
					Assert.AreEqual(1, tag.TagId);
					Assert.AreEqual(1, mediaFileTag.TagId);
					Assert.AreEqual(2, mediaFileTag.MediaFileId);

					// 追加済みのタグは無視される
					media2.AddTag("tag");

					Assert.AreEqual(1, media2.Tags.Count);
					Assert.AreEqual(1, db.MediaFileTags.Count());
					Assert.AreEqual(1, db.Tags.Count());

					media2.AddTag("tag2");
					media1.AddTag("tag2");
					media1.AddTag("tag");

					Assert.AreEqual(2, media2.Tags.Count);
					Assert.AreEqual(4, db.MediaFileTags.Count());
					Assert.AreEqual(4, db.Tags.Count());

					media2.RemoveTag("tag2");

					Assert.AreEqual(1, media2.Tags.Count);
					//タグとメディアのリレーションは消える
					Assert.AreEqual(3, db.MediaFileTags.Count());
					// 登録したタグは消えない
					Assert.AreEqual(4, db.Tags.Count());
					CollectionAssert.AreEquivalent(
						new[] {
							"[2],tag",
							"[1],tag2",
							"[1],tag"
						},
						db.MediaFileTags
							.Include(x => x.Tag)
							.ToList()
							.Select(x => $"[{x.MediaFileId}],{x.Tag.TagName}"));

					// 存在しないタグは何も起こらない
					media2.RemoveTag("tag2");
					Assert.AreEqual(1, media2.Tags.Count);
					Assert.AreEqual(3, db.MediaFileTags.Count());

					// もしIDがnull(=DB登録されていない状態)だったら何も起こらない
					media2.MediaFileId = null;
					media2.RemoveTag("tag");
					Assert.AreEqual(1, media2.Tags.Count);
					Assert.AreEqual(3, db.MediaFileTags.Count());
				}
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
