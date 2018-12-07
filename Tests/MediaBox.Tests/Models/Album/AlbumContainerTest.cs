using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBox.TestUtilities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using Unity;
using Unity.Lifetime;
using UnityContainer = Unity.UnityContainer;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumContainerTest {

		private static string _testDataDir;
		private static Dictionary<string, string> _testDirectories;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			_testDataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"TestData\");
			_testDirectories = new Dictionary<string, string> {
				{"0", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir0")},
				{"1", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir1")},
				{"sub", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"dir1\sub")},
				{"2", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir2")},
				{"3", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir3")},
				{"4", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir4")},
				{"5", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir5")},
				{"6", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dir6")}
			};
		}

		[SetUp]
		public void SetUp() {
			TypeRegistrations.RegisterType(new UnityContainer());

			var settings = Get.Instance<ISettings>();
			// DataBase
			var scsb = new SqliteConnectionStringBuilder {
				DataSource = settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(scsb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());
			dbContext.Database.EnsureDeleted();
			dbContext.Database.EnsureCreated();

			// Directory
			foreach (var dir in _testDirectories) {
				DirectoryUtility.DirectoryDelete(dir.Value);
			}
			foreach (var dir in _testDirectories) {
				Directory.CreateDirectory(dir.Value);
			}

			FileUtility.Copy(
				_testDataDir,
				_testDirectories["0"],
				Directory.GetFiles(_testDataDir).Select(Path.GetFileName));

			// サムネイルディレクトリ
			DirectoryUtility.DirectoryDelete(settings.PathSettings.ThumbnailDirectoryPath.Value);
			Directory.CreateDirectory(settings.PathSettings.ThumbnailDirectoryPath.Value);
		}

		[TearDown]
		public void TearDown() {
			var db = Get.Instance<MediaBoxDbContext>();
			db.Database.EnsureDeleted();
			// Directory
			foreach (var dir in _testDirectories) {
				DirectoryUtility.DirectoryDelete(dir.Value);
			}
			UnityConfig.UnityContainer.Dispose();
		}

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
				Assert.AreEqual(count, container.AlbumList.Count);
				CollectionAssert.AreEquivalent(Enumerable.Range(1, count),
				container.AlbumList.Cast<RegisteredAlbum>().Select(x => x.AlbumId));
			}
		}

		[Test]
		public void SetAlbumToCurrent() {
			using (var album1 = Get.Instance<RegisteredAlbum>())
			using (var album2 = Get.Instance<RegisteredAlbum>())
			using (var container = Get.Instance<AlbumContainer>()) {
				container.SetAlbumToCurrent(album1);
				Assert.AreEqual(album1,container.CurrentAlbum.Value);
				container.SetAlbumToCurrent(album2);
				Assert.AreEqual(album2, container.CurrentAlbum.Value);
			}
		}

		[Test]
		public void SetTemporaryAlbumToCurrent() {
			using (var container = Get.Instance<AlbumContainer>()) {
				container.TemporaryAlbumPath.Value = _testDirectories["0"];
				container.TemporaryAlbumPath.Value = _testDirectories["1"];
				Assert.IsNull(container.CurrentAlbum.Value);
				container.SetTemporaryAlbumToCurrent();
				CollectionAssert.AreEqual(
					new[] {
						_testDirectories["1"]

					}, container.CurrentAlbum.Value?.MonitoringDirectories);
				container.TemporaryAlbumPath.Value = _testDirectories["3"];
				container.SetTemporaryAlbumToCurrent();
				CollectionAssert.AreEqual(
					new[] {
						_testDirectories["3"]

					}, container.CurrentAlbum.Value?.MonitoringDirectories);
			}
		}


		[Test]
		public async Task DeleteAlbum() {
			var db = Get.Instance<MediaBoxDbContext>();
			for (var i = 0; i < 3; i++) {
				using (var album = Get.Instance<RegisteredAlbumForTest>()) {
					album.Create();
					await album.CallOnAddedItemAsync(Get.Instance<MediaFile>(Path.Combine(_testDirectories["0"], $"image{i + 1}.jpg")));
					album.MonitoringDirectories.Add(_testDirectories[$"{i}"]);
				}
			}

			using (var container = Get.Instance<AlbumContainer>()) {
				Assert.AreEqual(3, container.AlbumList.Count);
				using (var album = container.AlbumList.First()) {
					var count =
						db.Albums
							.Include(x => x.AlbumMediaFiles)
							.ThenInclude(x => x.MediaFile)
							.Include(x => x.AlbumDirectories)
							.Count(x => x.AlbumId == 1);
					Assert.AreEqual(1, count);
					Assert.AreEqual(3, db.AlbumDirectories.Count());
					Assert.AreEqual(3, db.AlbumMediaFiles.Count());
					Assert.AreEqual(3, db.Albums.Count());
					Assert.AreEqual(true, container.AlbumList.Any(x => x == album));
					container.DeleteAlbum((RegisteredAlbum)album);
					count =
						db.Albums
							.Include(x => x.AlbumMediaFiles)
							.ThenInclude(x => x.MediaFile)
							.Include(x => x.AlbumDirectories)
							.Count(x => x.AlbumId == 1);
					Assert.AreEqual(0, count);
					Assert.AreEqual(2, db.AlbumDirectories.Count());
					Assert.AreEqual(2, db.AlbumMediaFiles.Count());
					Assert.AreEqual(2, db.Albums.Count());
					Assert.AreEqual(false, container.AlbumList.Any(x => x == album));
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
