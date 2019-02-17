using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumTest : TestClassBase {
		[Test]
		public void Title() {
			using (var album = Get.Instance<AlbumForTest>()) {
				album.Title.Value = "bear";
				album.Title.Value.Is("bear");
				album.Title.Value = "lion";
				album.Title.Value.Is("lion");
			}
		}

		[Test]
		public void MonitoringDirectories() {
			using (var album = Get.Instance<AlbumForTest>()) {
				album.MonitoringDirectories.Count.Is(0);
				album.MonitoringDirectories.Add(TestDirectories["0"]);
				album.MonitoringDirectories.Is(TestDirectories["0"]);

				album.MonitoringDirectories.Add(TestDirectories["1"]);
				album.MonitoringDirectories.Is(
					TestDirectories["0"],
					TestDirectories["1"]);

				album.MonitoringDirectories.AddRange(new[] {
					TestDirectories["2"],
					TestDirectories["4"]
				});
				album.MonitoringDirectories.Is(
					TestDirectories["0"],
					TestDirectories["1"],
					TestDirectories["2"],
					TestDirectories["4"]);

				album.MonitoringDirectories.Add(TestDirectories["sub"]);
				album.MonitoringDirectories.Is(
					TestDirectories["0"],
					TestDirectories["1"],
					TestDirectories["2"],
					TestDirectories["4"],
					TestDirectories["sub"]
				);

				album.MonitoringDirectories.AddRange(new[] {
					TestDirectories["5"],
					TestDirectories["6"] });

				album.MonitoringDirectories.Is(
					TestDirectories["0"],
					TestDirectories["1"],
					TestDirectories["2"],
					TestDirectories["4"],
					TestDirectories["sub"],
					TestDirectories["5"],
					TestDirectories["6"]
				);
			}
		}

		[Test]
		public void Items() {
			using (var album = Get.Instance<AlbumForTest>()) {
				album.Map.Value.Items.Count.Is(0);

				var item1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
				album.Items.Add(item1);

				album.Map.Value.Items.Count.Is(1);
				album.Map.Value.Items[0].Is(item1);
			}
		}

		[Test]
		public async Task FileSystemWatcher() {
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.ImageExtensions.Clear();
			settings.GeneralSettings.ImageExtensions.Add(".jpg");
			using (var album = Get.Instance<AlbumForTest>()) {
				album.LoadFileInDirectoryArgs.Count.Is(0);
				album.OnFileSystemEventArgs.Count.Is(0);

				album.LoadFileInDirectoryArgs.Count.Is(0);

				// 存在するディレクトリならディレクトリ読み込みが発生して監視が始まる
				album.MonitoringDirectories.Add(TestDirectories["1"]);

				await Task.Delay(100);

				album.LoadFileInDirectoryArgs.Count.Is(1);
				album.LoadFileInDirectoryArgs[0].Is(TestDirectories["1"]);

				FileUtility.Copy(TestDataDir, TestDirectories["1"], new[] {
					"image1.jpg",
					"image2.jpg"
				});

				await Task.Delay(100);

				var createdEventArgs = album.OnFileSystemEventArgs.Where(x => x.ChangeType == WatcherChangeTypes.Created);
				createdEventArgs.Count().Is(2);
				createdEventArgs.Select(x => x.FullPath).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"));

				// 2回目もOK
				FileUtility.Copy(TestDataDir, TestDirectories["1"], new[] {
					"image4.jpg"
				});

				await Task.Delay(100);

				createdEventArgs.Count().Is(3);
				createdEventArgs.Select(x => x.FullPath).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"));

				// サブディレクトリもOK
				FileUtility.Copy(TestDataDir, TestDirectories["sub"], new[] {
					"image8.jpg"
				});

				await Task.Delay(100);

				createdEventArgs.Count().Is(4);
				createdEventArgs.Select(x => x.FullPath).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["sub"], "image8.jpg"));

				// 監視から外すとファイル追加されない
				album.MonitoringDirectories.Remove(TestDirectories["1"]);

				await Task.Delay(100);

				FileUtility.Copy(TestDataDir, TestDirectories["1"], new[] {
					"image5.jpg"
				});

				await Task.Delay(500);

				createdEventArgs.Count().Is(4);
				createdEventArgs.Select(x => x.FullPath).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["sub"], "image8.jpg"));
			}
		}

		[Test]
		public async Task CurrentMediaFile() {

			var settings = Get.Instance<ISettings>();
			using (var album = Get.Instance<AlbumForTest>()) {
				var image1 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
				var image2 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
				var image3 = (ImageFileModel)this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Detail;
				await Task.Delay(10);
				image1.Image.IsNull();
				image2.Image.IsNull();
				image3.Image.IsNull();

				album.CurrentMediaFile.Value = image1;

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => image1.Image != null)
					.Timeout(TimeSpan.FromSeconds(1))
					.FirstAsync();

				image1.Image.IsNotNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image1);

				album.CurrentMediaFile.Value = image2;

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => image2.Image != null)
					.Timeout(TimeSpan.FromSeconds(1))
					.FirstAsync();

				image1.Image.IsNull();
				image2.Image.IsNotNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image2);

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Library;
				await Task.Delay(10);

				album.CurrentMediaFile.Value = image3;

				await Task.Delay(1000);

				image2.Image.IsNull();
				image3.Image.IsNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image3);

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Map;
				await Task.Delay(10);

				album.CurrentMediaFile.Value = image1;

				await Task.Delay(1000);

				image3.Image.IsNull();
				image1.Image.IsNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image1);
			}
		}

		[Test]
		public void CurrentMediaFiles() {
			using (var album = Get.Instance<AlbumForTest>()) {
				album.CurrentMediaFiles.Value.Count().Is(0);
				album.MediaFileInformations.Value.Files.Value.Count().Is(0);

				var item1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
				album.CurrentMediaFiles.Value = new[] { item1 };

				album.CurrentMediaFiles.Value.Count().Is(1);
				album.MediaFileInformations.Value.Files.Value.Count().Is(1);
				album.CurrentMediaFiles.Value.First().Is(item1);
				album.MediaFileInformations.Value.Files.Value.First().Is(item1);
			}
		}

		[TestCase(DisplayMode.Detail)]
		[TestCase(DisplayMode.Library)]
		[TestCase(DisplayMode.Map)]
		public async Task ChangeDisplayMode(DisplayMode mode) {
			using (var album = Get.Instance<AlbumForTest>()) {
				var settings = Get.Instance<ISettings>();
				album.ChangeDisplayMode(mode);
				await Task.Delay(10);
				album.DisplayMode.Value.Is(mode);
				settings.GeneralSettings.DisplayMode.Value.Is(mode);
			}
		}

		private class AlbumForTest : AlbumModel {
			public readonly List<string> LoadFileInDirectoryArgs = new List<string>();

			public readonly List<FileSystemEventArgs> OnFileSystemEventArgs = new List<FileSystemEventArgs>();

			protected override void LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
				this.LoadFileInDirectoryArgs.Add(directoryPath);
			}

			protected override void OnFileSystemEvent(FileSystemEventArgs e) {
				this.OnFileSystemEventArgs.Add(e);
			}
		}
	}
}
