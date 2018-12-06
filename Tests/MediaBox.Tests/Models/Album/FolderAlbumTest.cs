using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MediaBox.TestUtilities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Reactive.Bindings;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class FolderAlbumTest {

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
			foreach (var dir in _testDirectories) {
				DirectoryUtility.DirectoryDelete(dir.Value);
			}
		}

		[Test]
		public async Task LoadFileInDirectory() {
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.TargetExtensions.Value = new[] { ".jpg" };

			FileUtility.Copy(
				_testDirectories["0"],
				_testDirectories["1"],
				new[] { "image1.jpg", "image2.jpg", "image4.jpg", "image6.jpg", "image8.jpg", "image9.png" });


			FileUtility.Copy(
				_testDirectories["0"],
				_testDirectories["sub"],
				new[] { "image3.jpg", "image7.jpg" });


			FileUtility.Copy(
				_testDirectories["0"],
				_testDirectories["2"],
				new[] { "image5.jpg" });

			using (var album1 = Get.Instance<FolderAlbumForTest>(_testDirectories["1"])) {

				await Task.Delay(100);

				Assert.AreEqual(7, album1.Items.Count);
				CollectionAssert.AreEqual(new[] {
					Path.Combine(_testDirectories["1"], "image1.jpg"),
					Path.Combine(_testDirectories["1"], "image2.jpg"),
					Path.Combine(_testDirectories["1"], "image4.jpg"),
					Path.Combine(_testDirectories["1"], "image6.jpg"),
					Path.Combine(_testDirectories["1"], "image8.jpg"),
					Path.Combine(_testDirectories["sub"], "image3.jpg"),
					Path.Combine(_testDirectories["sub"], "image7.jpg")
				}, album1.Items.Select(x => x.FilePath.Value));

				// 2回目実行しても同じファイルは追加されない
				album1.CallLoadFileInDirectory(_testDirectories["1"]);
				await Task.Delay(100);
				Assert.AreEqual(7, album1.Items.Count);
				
				// 存在しないフォルダの場合は何も起こらない
				album1.CallLoadFileInDirectory($"{_testDirectories["1"]}____");
				await Task.Delay(100);
				Assert.AreEqual(7, album1.Items.Count);
			}
		}

		[Test]
		public void FolderAlbum() {
			using (var album = Get.Instance<FolderAlbumForTest>(_testDirectories["0"])) {
				Assert.AreEqual(1, album.MonitoringDirectories.Count);
				CollectionAssert.AreEqual(
					new[] {
						_testDirectories["0"]

					},album.MonitoringDirectories);
				Assert.AreEqual(_testDirectories["0"], album.Title.Value);
			}
		}

		[Test]
		public async Task OnAddedItemAsync() {
			using (var album1 = Get.Instance<FolderAlbumForTest>(_testDirectories["1"])) {
				using (var media1 = Get.Instance<MediaFile>(Path.Combine(_testDirectories["0"], "image1.jpg")))
				using (var media2 = Get.Instance<MediaFile>(Path.Combine(_testDirectories["0"], "image2.jpg"))) {
					var thumbDir = Get.Instance<ISettings>().PathSettings.ThumbnailDirectoryPath.Value;
					Assert.IsNull(media1.MediaFileId);
					Assert.IsNull(media1.Exif.Value);
					Assert.IsNull(media1.Thumbnail.Value);
					Assert.AreEqual(0, Directory.GetFiles(thumbDir).Length);

					await album1.CallOnAddedItemAsync(media1);
					Assert.IsNull(media1.MediaFileId);
					Assert.IsNotNull(media1.Exif.Value);
					Assert.IsNotNull(media1.Thumbnail.Value);
					Assert.AreEqual(0, Directory.GetFiles(thumbDir).Length);
					Assert.AreEqual(35.6517139, media1.Latitude.Value, 0.00001);
					Assert.AreEqual(136.821275, media1.Longitude.Value, 0.00001);
					Assert.AreEqual(1, media1.Orientation.Value);
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

			public FolderAlbumForTest(string path) : base(path)
			{
			}
		}
	}
}
