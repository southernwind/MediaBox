using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumContainerTest : TestClassBase {

		[TestCase(0)]
		[TestCase(1)]
		[TestCase(5)]
		public void AlbumContainer(int count) {
			for (var i = 0; i < count; i++) {
				using (var album = Get.Instance<RegisteredAlbum>()) {
					album.Create();
				}
			}

			using (var container = Get.Instance<AlbumContainer>()) {
				container.AlbumList.Count.Is(count);
				container.AlbumList.Cast<RegisteredAlbum>().Select(x => x.AlbumId).Is(Enumerable.Range(1, count));
			}
		}

		[Test]
		public void SetAlbumToCurrent() {
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>())
			using (var container = Get.Instance<AlbumContainer>()) {
				container.SetAlbumToCurrent(album1);
				album1.Is(container.CurrentAlbum.Value);
				container.SetAlbumToCurrent(album2);
				album2.Is(container.CurrentAlbum.Value);
			}
		}

		[Test]
		public void SetTemporaryAlbumToCurrent() {
			using (var container = Get.Instance<AlbumContainer>()) {
				container.SetTemporaryAlbumToCurrent();
				container.CurrentAlbum.Value.IsNull();

				container.TemporaryAlbumPath.Value = TestDirectories["0"];
				container.TemporaryAlbumPath.Value = TestDirectories["1"];
				container.CurrentAlbum.Value.IsNull();
				container.SetTemporaryAlbumToCurrent();
				container.CurrentAlbum.Value.MonitoringDirectories.Is(TestDirectories["1"]);
				container.TemporaryAlbumPath.Value = TestDirectories["3"];
				container.SetTemporaryAlbumToCurrent();
				container.CurrentAlbum.Value.MonitoringDirectories.Is(TestDirectories["3"]);
			}
		}


		[Test]
		public async Task DeleteAlbum() {
			var db = Get.Instance<MediaBoxDbContext>();
			for (var i = 0; i < 3; i++) {
				using (var album = Get.Instance<RegisteredAlbumForTest>()) {
					album.Create();
					await album.CallOnAddedItemAsync(this.MediaFactory.Create(Path.Combine(TestDirectories["0"], $"image{i + 1}.jpg")));
					album.MonitoringDirectories.Add(TestDirectories[$"{i}"]);
				}
			}

			using (var container = Get.Instance<AlbumContainer>()) {
				container.AlbumList.Count.Is(3);
				using (var album = container.AlbumList.First()) {
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
					container.AlbumList.Any(x => x == album).IsTrue();
					container.DeleteAlbum((RegisteredAlbum)album);
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
					container.AlbumList.Any(x => x == album).IsFalse();
				}
			}
		}

		/// <summary>
		/// protectedメソッドを呼び出すためのテスト用クラス
		/// </summary>
		private class RegisteredAlbumForTest : RegisteredAlbum {
			public async Task CallOnAddedItemAsync(MediaFile mediaFile) {
				await this.OnAddedItemAsync(mediaFile);
			}
		}
	}
}
