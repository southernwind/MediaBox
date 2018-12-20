using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumCreatorTest : TestClassBase {
		[Test]
		public void CreateAlbum() {
			var creator = Get.Instance<AlbumCreator>();
			var albumContainer = Get.Instance<AlbumContainer>();

			Assert.AreEqual(0, albumContainer.AlbumList.Count);
			Assert.IsNull(albumContainer.CurrentAlbum.Value);
			creator.CreateAlbum();
			Assert.AreEqual(1, albumContainer.AlbumList.Count);
			Assert.AreEqual(1, creator.Album.Value.AlbumId);
			creator.CreateAlbum();
			Assert.AreEqual(2, albumContainer.AlbumList.Count);
			Assert.AreEqual(2, creator.Album.Value.AlbumId);
		}

		[Test]
		public void EditAlbum() {
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>()) {
				var creator = Get.Instance<AlbumCreator>();
				Assert.IsNull(creator.Album.Value);
				creator.EditAlbum(album1);
				Assert.AreEqual(album1, creator.Album.Value);
				creator.EditAlbum(album2);
				Assert.AreEqual(album2, creator.Album.Value);
			}
		}

		[Test]
		public async Task AddFiles() {
			using (var album1 = Get.Instance<RegisteredAlbum>()) {
				album1.Create();
				var creator = Get.Instance<AlbumCreator>();
				creator.EditAlbum(album1);
				Assert.AreEqual(0, album1.Items.Count);
				var media1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				var media2 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg"));

				creator.AddFiles(new []{media1});
				await Task.Delay(100);
				Assert.AreEqual(1, album1.Items.Count);
				CollectionAssert.AreEqual(new[] { media1 }, album1.Items);

				creator.AddFiles(new []{media2});
				await Task.Delay(100);
				Assert.AreEqual(2, album1.Items.Count);
				CollectionAssert.AreEqual(new[] { media1, media2 }, album1.Items);
			}
		}

		[Test]
		public void AddDirectory() {
			using (var album1 = Get.Instance<RegisteredAlbum>()) {
				album1.Create();
				var creator = Get.Instance<AlbumCreator>();
				creator.EditAlbum(album1);
				Assert.AreEqual(0, album1.MonitoringDirectories.Count);
				
				creator.AddDirectory(TestDirectories["0"]);
				Assert.AreEqual(1, album1.MonitoringDirectories.Count);
				CollectionAssert.AreEqual(new[] {
					TestDirectories["0"]
				}, album1.MonitoringDirectories);

				creator.AddDirectory(TestDirectories["1"]);
				Assert.AreEqual(2, album1.MonitoringDirectories.Count);
				CollectionAssert.AreEqual(new[] {
					TestDirectories["0"],
					TestDirectories["1"]
				}, album1.MonitoringDirectories);
			}
		}
	}
}
