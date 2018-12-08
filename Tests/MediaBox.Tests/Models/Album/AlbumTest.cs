using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using SandBeige.MediaBox.TestUtilities;
using NUnit.Framework;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Repository;
using SandBeige.MediaBox.Utilities;
using Unity;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumTest : TestClassBase {
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
				album.MonitoringDirectories.Add(TestDirectories["0"]);
				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["0"]
				}, album.MonitoringDirectories);

				album.MonitoringDirectories.Add(TestDirectories["1"]);
				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["0"],
					TestDirectories["1"]
				}, album.MonitoringDirectories);

				album.MonitoringDirectories.AddRange(new[] {
					TestDirectories["2"],
					TestDirectories["4"]
				});
				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["0"],
					TestDirectories["1"],
					TestDirectories["2"],
					TestDirectories["4"]
				}, album.MonitoringDirectories);

				album.MonitoringDirectories.Add(TestDirectories["sub"]);
				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["0"],
					TestDirectories["1"],
					TestDirectories["2"],
					TestDirectories["4"],
					TestDirectories["sub"]
				}, album.MonitoringDirectories);


				album.MonitoringDirectories.AddRangeOnScheduler(
					TestDirectories["5"],
					TestDirectories["6"]);
				CollectionAssert.AreEquivalent(new[] {
					TestDirectories["0"],
					TestDirectories["1"],
					TestDirectories["2"],
					TestDirectories["4"],
					TestDirectories["sub"],
					TestDirectories["5"],
					TestDirectories["6"]
				}, album.MonitoringDirectories);
			}
		}

		[Test]
		public async Task Items() {
			using (var album = Get.Instance<AlbumForTest>()) {
				Assert.AreEqual(0, album.OnAddedItemAsyncArgs.Count);

				var item1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
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
				album.MonitoringDirectories.Add($"{TestDirectories["1"]}____");
				Assert.AreEqual(1, log.LogList.Select(x => x.LogLevel == LogLevel.Warning).Count());

				Assert.AreEqual(0, album.LoadFileInDirectoryArgs.Count);

				// 存在するディレクトリならディレクトリ読み込みが発生して監視が始まる
				album.MonitoringDirectories.Add(TestDirectories["1"]);

				Assert.AreEqual(1,album.LoadFileInDirectoryArgs.Count);
				Assert.AreEqual(TestDirectories["1"], album.LoadFileInDirectoryArgs[0]);

				FileUtility.Copy(TestDirectories["0"], TestDirectories["1"],new [] {
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
				FileUtility.Copy(TestDirectories["0"], TestDirectories["1"], new[] {
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
				FileUtility.Copy(TestDirectories["0"], TestDirectories["sub"], new[] {
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
				album.MonitoringDirectories.Remove(TestDirectories["1"]);

				FileUtility.Copy(TestDirectories["0"], TestDirectories["1"], new[] {
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
