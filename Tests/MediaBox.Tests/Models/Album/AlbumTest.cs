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
using SandBeige.MediaBox.Composition.Enum;

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
		public async Task MonitoringDirectories() {
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
				await Task.Delay(10);
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
				Assert.AreEqual(0, album.Map.Value.Items.Count);

				var item1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				album.Items.Add(item1);
				Assert.AreEqual(0, album.OnAddedItemAsyncArgs.Count);

				DispatcherUtility.DoEvents();

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => album.Count.Value != 0)
					.Timeout(TimeSpan.FromSeconds(0.5))
					.FirstAsync();

				Assert.AreEqual(1, album.OnAddedItemAsyncArgs.Count);
				Assert.AreEqual(1, album.Map.Value.Items.Count);
				Assert.AreEqual(item1, album.OnAddedItemAsyncArgs[0]);
				Assert.AreEqual(item1, album.Map.Value.Items[0]);
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
				CollectionAssert.AreEquivalent(new [] {
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
				CollectionAssert.AreEquivalent(new[] {
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
				CollectionAssert.AreEquivalent(new[] {
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
				CollectionAssert.AreEquivalent(new[] {
					"image1.jpg",
					"image2.jpg",
					"image4.jpg",
					"image8.jpg"
				}, album.Items.Select(x => x.FileName.Value));
			}
		}

		[Test]
		public async Task CurrentMediaFile() {
			var settings = Get.Instance<ISettings>();
			using (var album = Get.Instance<AlbumForTest>()) {
				var image1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				var image2 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image2.jpg"));
				var image3 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image3.jpg"));

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Detail;
				Assert.IsNull(image1.Image.Value);
				Assert.IsNull(image2.Image.Value);
				Assert.IsNull(image3.Image.Value);

				Assert.IsNull(image1.Exif.Value);
				album.CurrentMediaFile.Value = image1;

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => image1.Image.Value != null)
					.Timeout(TimeSpan.FromSeconds(1))
					.FirstAsync();

				Assert.IsNotNull(image1.Image.Value);
				Assert.AreEqual(image1, album.Map.Value.CurrentMediaFile.Value);
				Assert.IsNotNull(image1.Exif.Value);

				album.CurrentMediaFile.Value = image2;

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => image2.Image.Value != null)
					.Timeout(TimeSpan.FromSeconds(1))
					.FirstAsync();

				Assert.IsNull(image1.Image.Value);
				Assert.IsNotNull(image2.Image.Value);
				Assert.AreEqual(image2, album.Map.Value.CurrentMediaFile.Value);

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Library;

				album.CurrentMediaFile.Value = image3;

				await Task.Delay(1000);

				Assert.IsNull(image2.Image.Value);
				Assert.IsNull(image3.Image.Value);
				Assert.AreEqual(image3, album.Map.Value.CurrentMediaFile.Value);

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Map;

				album.CurrentMediaFile.Value = image1;

				await Task.Delay(1000);

				Assert.IsNull(image3.Image.Value);
				Assert.IsNull(image1.Image.Value);
				Assert.AreEqual(image1, album.Map.Value.CurrentMediaFile.Value);
			}
		}

		[Test]
		public void CurrentMediaFiles() {
			using (var album = Get.Instance<AlbumForTest>()) {
				Assert.AreEqual(0, album.CurrentMediaFiles.Count);
				Assert.AreEqual(0, album.MediaFileProperties.Value.Items.Count);

				var item1 = Get.Instance<MediaFile>(Path.Combine(TestDirectories["0"], "image1.jpg"));
				album.CurrentMediaFiles.Add(item1);

				Assert.AreEqual(1, album.CurrentMediaFiles.Count);
				Assert.AreEqual(1, album.MediaFileProperties.Value.Items.Count);
				Assert.AreEqual(item1, album.CurrentMediaFiles[0]);
				Assert.AreEqual(item1, album.MediaFileProperties.Value.Items[0]);
			}
		}

		[TestCase(DisplayMode.Detail)]
		[TestCase(DisplayMode.Library)]
		[TestCase(DisplayMode.Map)]
		public void ChangeDisplayMode(DisplayMode mode) {
			using (var album = Get.Instance<AlbumForTest>()) {
				var settings = Get.Instance<ISettings>();
				album.ChangeDisplayMode(mode);
				Assert.AreEqual(mode, album.DisplayMode.Value);
				Assert.AreEqual(mode, settings.GeneralSettings.DisplayMode.Value);
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
