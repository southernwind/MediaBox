using System.IO;
using System.Linq;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumSelectorTest : TestClassBase {

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void AlbumContainer(int count) {
			for (var i = 0; i < count; i++) {
				using (var album = Get.Instance<RegisteredAlbum>()) {
					album.Create();
					album.AlbumPath.Value = "/iphone";
					album.ReflectToDataBase();
				}
			}

			using (var selector = Get.Instance<AlbumSelector>()) {
				selector.AlbumList.Count.Is(count);
				selector.AlbumList.Cast<RegisteredAlbum>().Select(x => x.AlbumId.Value).Is(Enumerable.Range(1, count));
			}
		}

		[Test]
		public void SetAlbumToCurrent() {
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>())
			using (var selector = Get.Instance<AlbumSelector>()) {
				selector.SetAlbumToCurrent(album1);
				album1.Is(selector.CurrentAlbum.Value);
				selector.SetAlbumToCurrent(album2);
				album2.Is(selector.CurrentAlbum.Value);
			}
		}

		[Test]
		public void SetTemporaryAlbumToCurrent() {
			using (var selector = Get.Instance<AlbumSelector>()) {
				selector.SetTemporaryAlbumToCurrent();
				selector.CurrentAlbum.Value.IsNull();

				selector.TemporaryAlbumPath.Value = TestDirectories["0"];
				selector.TemporaryAlbumPath.Value = TestDirectories["1"];
				selector.CurrentAlbum.Value.IsNull();
				selector.SetTemporaryAlbumToCurrent();
				selector.CurrentAlbum.Value.MonitoringDirectories.Is(TestDirectories["1"]);
				selector.TemporaryAlbumPath.Value = TestDirectories["3"];
				selector.SetTemporaryAlbumToCurrent();
				selector.CurrentAlbum.Value.MonitoringDirectories.Is(TestDirectories["3"]);
			}
		}


		[Test]
		public void DeleteAlbum() {
			var db = Get.Instance<MediaBoxDbContext>();
			for (var i = 0; i < 3; i++) {
				using (var album = Get.Instance<RegisteredAlbum>()) {
					album.Create();
					album.AlbumPath.Value = "/iphone/picture";
					album.AddFiles(new[]{
						this.MediaFactory.Create(Path.Combine(TestDataDir, $"image{i + 1}.jpg"))
					});
					album.MonitoringDirectories.Add(TestDirectories[$"{i + 1}"]);
					album.ReflectToDataBase();
				}
			}

			using (var selector = Get.Instance<AlbumSelector>()) {
				selector.AlbumList.Count.Is(3);
				var album = selector.AlbumList.First();
				var count =
					db.Albums
						.Include(x => x.AlbumMediaFiles)
						.ThenInclude(x => x.MediaFile)
						.Include(x => x.AlbumDirectories)
						.Count(x => x.AlbumId == 1);
				count.Is(1);
				db.AlbumDirectories.Count().Is(3);
				db.AlbumMediaFiles.Count().Is(3);
				db.Albums.Count().Is(3);
				selector.AlbumList.Any(x => x == album).IsTrue();
				selector.DeleteAlbum(album);
				count =
					db.Albums
						.Include(x => x.AlbumMediaFiles)
						.ThenInclude(x => x.MediaFile)
						.Include(x => x.AlbumDirectories)
						.Count(x => x.AlbumId == 1);
				count.Is(0);
				db.AlbumDirectories.Count().Is(2);
				db.AlbumMediaFiles.Count().Is(2);
				db.Albums.Count().Is(2);
				selector.AlbumList.Any(x => x == album).IsFalse();
			}
		}
	}
}
