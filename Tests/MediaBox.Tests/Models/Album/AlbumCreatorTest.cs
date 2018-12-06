using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaBox.TestUtilities;
using Microsoft.Data.Sqlite;
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
	public class AlbumCreatorTest {

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
		public void AddFile() {
			using (var album1 = Get.Instance<RegisteredAlbum>()) {
				album1.Create();
				var creator = Get.Instance<AlbumCreator>();
				creator.EditAlbum(album1);
				Assert.AreEqual(0, album1.Items.Count);
				var media1 = Get.Instance<MediaFile>(Path.Combine(_testDirectories["0"], "image1.jpg"));
				var media2 = Get.Instance<MediaFile>(Path.Combine(_testDirectories["0"], "image2.jpg"));

				creator.AddFile(media1);
				Assert.AreEqual(1, album1.Items.Count);
				CollectionAssert.AreEqual(new[] { media1 }, album1.Items);

				creator.AddFile(media2);
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
				
				creator.AddDirectory(_testDirectories["0"]);
				Assert.AreEqual(1, album1.MonitoringDirectories.Count);
				CollectionAssert.AreEqual(new[] {
					_testDirectories["0"]
				}, album1.MonitoringDirectories);

				creator.AddDirectory(_testDirectories["1"]);
				Assert.AreEqual(2, album1.MonitoringDirectories.Count);
				CollectionAssert.AreEqual(new[] {
					_testDirectories["0"],
					_testDirectories["1"]
				}, album1.MonitoringDirectories);
			}
		}
	}
}
