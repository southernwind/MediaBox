using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Livet;
using MediaBox.TestUtilities;
using Microsoft.Data.Sqlite;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumTest {
		private static string _testDataDir;
		private static Dictionary<string, string> _testDirectories;

		[OneTimeSetUp]
		public void OneTimeSetUp() {
			DispatcherHelper.UIDispatcher = Dispatcher.CurrentDispatcher;
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
			var sb = new SqliteConnectionStringBuilder {
				DataSource = settings.PathSettings.DataBaseFilePath.Value
			};
			var dbContext = new MediaBoxDbContext(new SqliteConnection(sb.ConnectionString));
			UnityConfig.UnityContainer.RegisterInstance(dbContext, new ContainerControlledLifetimeManager());

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
		}

		[TearDown]
		public void TearDown() {
			foreach (var dir in _testDirectories) {
				DirectoryUtility.DirectoryDelete(dir.Value);
			}
		}

		[Test]
		public void Title() {
			using (var album = Get.Instance<AlbumForTest>()) {
				album.Title.Value = "bear";
				Assert.AreEqual("bear", album.Title.Value);
				album.Title.Value = "lion";
				Assert.AreEqual("lion", album.Title.Value);
			}
		}

		[Test]
		public void MonitoringDirectories() {
			using (var album = Get.Instance<AlbumForTest>()) {
				Assert.AreEqual(0, album.MonitoringDirectories.Count);
				album.MonitoringDirectories.Add(_testDirectories["0"]);
				CollectionAssert.AreEquivalent(new[] {
					_testDirectories["0"]
				}, album.MonitoringDirectories);

				album.MonitoringDirectories.Add(_testDirectories["1"]);
				CollectionAssert.AreEquivalent(new[] {
					_testDirectories["0"],
					_testDirectories["1"]
				}, album.MonitoringDirectories);

				album.MonitoringDirectories.AddRange(new[] {
					_testDirectories["2"],
					_testDirectories["4"]
				});
				CollectionAssert.AreEquivalent(new[] {
					_testDirectories["0"],
					_testDirectories["1"],
					_testDirectories["2"],
					_testDirectories["4"]
				}, album.MonitoringDirectories);

				album.MonitoringDirectories.Add(_testDirectories["sub"]);
				CollectionAssert.AreEquivalent(new[] {
					_testDirectories["0"],
					_testDirectories["1"],
					_testDirectories["2"],
					_testDirectories["4"],
					_testDirectories["sub"]
				}, album.MonitoringDirectories);


				album.MonitoringDirectories.AddRangeOnScheduler(
					_testDirectories["5"],
					_testDirectories["6"]);
				CollectionAssert.AreEquivalent(new[] {
					_testDirectories["0"],
					_testDirectories["1"],
					_testDirectories["2"],
					_testDirectories["4"],
					_testDirectories["sub"],
					_testDirectories["5"],
					_testDirectories["6"]
				}, album.MonitoringDirectories);
			}
		}

		[Test]
		public async Task Items() {
			using (var album = Get.Instance<AlbumForTest>()) {
				Assert.AreEqual(0, album.OnAddedItemAsyncArgs.Count);

				var item1 = Get.Instance<MediaFile>(Path.Combine(_testDirectories["0"], "image1.jpg"));
				album.Items.Add(item1);
				Assert.AreEqual(0, album.Count.Value);
				Assert.AreEqual(0, album.OnAddedItemAsyncArgs.Count);

				DispatcherUtility.DoEvents();

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => album.Count.Value != 0)
					.Timeout(TimeSpan.FromSeconds(0.5))
					.FirstAsync();

				Assert.AreEqual(1, album.Count.Value);
				Assert.AreEqual(1, album.OnAddedItemAsyncArgs.Count);
				Assert.AreEqual(item1, album.OnAddedItemAsyncArgs[0]);
			}
		}

		[Test]
		public async Task FileSystemWatcher() {
			var settings = Get.Instance<ISettings>();
			var log = new Logging();
			UnityConfig.UnityContainer.RegisterInstance<ILogging>(log);
			settings.GeneralSettings.TargetExtensions.Value = new[] {".jpg"};
			using (var album = Get.Instance<AlbumForTest>()) {
				Assert.AreEqual(0, album.LoadFileInDirectoryArgs.Count);
				Assert.AreEqual(0, album.Items.Count);

				// 存在しないディレクトリならログが出力されて、ディレクトリ読み込みは発生しない
				Assert.AreEqual(0, log.LogList.Select(x => x.LogLevel == LogLevel.Warning).Count());
				album.MonitoringDirectories.Add($"{_testDirectories["1"]}____");
				Assert.AreEqual(1, log.LogList.Select(x => x.LogLevel == LogLevel.Warning).Count());

				Assert.AreEqual(0, album.LoadFileInDirectoryArgs.Count);

				// 存在するディレクトリならディレクトリ読み込みが発生して監視が始まる
				album.MonitoringDirectories.Add(_testDirectories["1"]);

				Assert.AreEqual(1,album.LoadFileInDirectoryArgs.Count);
				Assert.AreEqual(_testDirectories["1"], album.LoadFileInDirectoryArgs[0]);

				FileUtility.Copy(_testDataDir,_testDirectories["1"],new [] {
					"image1.jpg",
					"image2.jpg"
				});

				await Task.Delay(100);

				Assert.AreEqual(2, album.Items.Count);
				CollectionAssert.AreEqual(new [] {
					"image1.jpg",
					"image2.jpg"
				},album.Items.Select(x => x.FileName.Value));

				// 2回目もOK
				FileUtility.Copy(_testDataDir, _testDirectories["1"], new[] {
					"image4.jpg",
					"image9.png"
				});

				await Task.Delay(100);

				Assert.AreEqual(3, album.Items.Count);
				CollectionAssert.AreEqual(new[] {
					"image1.jpg",
					"image2.jpg",
					"image4.jpg"
				}, album.Items.Select(x => x.FileName.Value));

				// サブディレクトリもOK
				FileUtility.Copy(_testDataDir, _testDirectories["sub"], new[] {
					"image8.jpg"
				});

				await Task.Delay(100);

				Assert.AreEqual(4,album.Items.Count);
				CollectionAssert.AreEqual(new[] {
					"image1.jpg",
					"image2.jpg",
					"image4.jpg",
					"image8.jpg"
				}, album.Items.Select(x => x.FileName.Value));

				// 監視から外すとファイル追加されない
				album.MonitoringDirectories.Remove(_testDirectories["1"]);

				FileUtility.Copy(_testDataDir, _testDirectories["1"], new[] {
					"image5.jpg"
				});

				await Task.Delay(500);

				Assert.AreEqual(4, album.Items.Count);
				CollectionAssert.AreEqual(new[] {
					"image1.jpg",
					"image2.jpg",
					"image4.jpg",
					"image8.jpg"
				}, album.Items.Select(x => x.FileName.Value));
			}
		}

		private class AlbumForTest:MediaBox.Models.Album.Album {
			public readonly List<string> LoadFileInDirectoryArgs = new List<string>();

			public readonly List<MediaFile> OnAddedItemAsyncArgs = new List<MediaFile>();

			protected override void LoadFileInDirectory(string directoryPath) {
				this.LoadFileInDirectoryArgs.Add(directoryPath);
			}

			protected override Task OnAddedItemAsync(MediaFile mediaFile) {
				this.OnAddedItemAsyncArgs.Add(mediaFile);
				return new Task(() => {});
			}
		}
	}
}
