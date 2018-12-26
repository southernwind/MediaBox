using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumCreatorTest : TestClassBase {
		[Test]
		public void CreateAlbum() {
			var creator = Get.Instance<AlbumCreator>();
			var albumContainer = Get.Instance<AlbumContainer>();

			albumContainer.AlbumList.Count.Is(0);
			albumContainer.CurrentAlbum.Value.IsNull();
			creator.CreateAlbum();
			albumContainer.AlbumList.Count.Is(1);
			creator.Album.Value.AlbumId.Is(1);
			creator.CreateAlbum();
			albumContainer.AlbumList.Count.Is(2);
			creator.Album.Value.AlbumId.Is(2);
		}

		[Test]
		public void EditAlbum() {
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>()) {
				var creator = Get.Instance<AlbumCreator>();
				creator.Album.Value.IsNull();
				creator.EditAlbum(album1);
				creator.Album.Value.Is(album1);
				creator.EditAlbum(album2);
				creator.Album.Value.Is(album2);
			}
		}

		[Test]
		public async Task AddRemoveFiles() {
			using (var album1 = Get.Instance<RegisteredAlbum>()) {
				album1.Create();
				var creator = Get.Instance<AlbumCreator>();
				creator.EditAlbum(album1);
				album1.Items.Count.Is(0);
				var media1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
				var media2 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image2.jpg"));

				creator.AddFiles(new[] { media1 });
				await Task.Delay(100);
				album1.Items.Count.Is(1);
				album1.Items.Is(media1);

				creator.AddFiles(new[] { media2 });
				await Task.Delay(100);
				album1.Items.Count.Is(2);
				album1.Items.Is(media1, media2);

				creator.RemoveFiles(new[] { media1 });
				await Task.Delay(100);
				album1.Items.Count.Is(1);
				album1.Items.Is(media2);
			}
		}

		[Test]
		public void AddDirectory() {
			using (var album1 = Get.Instance<RegisteredAlbum>()) {
				album1.Create();
				var creator = Get.Instance<AlbumCreator>();
				creator.EditAlbum(album1);
				album1.MonitoringDirectories.Count.Is(0);

				creator.AddDirectory(TestDirectories["0"]);
				album1.MonitoringDirectories.Count.Is(1);
				album1.MonitoringDirectories.Is(TestDirectories["0"]);

				creator.AddDirectory(TestDirectories["1"]);
				album1.MonitoringDirectories.Count.Is(2);
				album1.MonitoringDirectories.Is(
					TestDirectories["0"],
					TestDirectories["1"]
				);
			}
		}
	}
}
