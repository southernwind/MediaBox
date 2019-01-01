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
				album.OnAddedItemArgs.Count.Is(0);
				album.Map.Value.Items.Count.Is(0);

				var item1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
				album.Items.Add(item1);

				album.OnAddedItemArgs.Count.Is(1);
				album.Map.Value.Items.Count.Is(1);
				album.OnAddedItemArgs[0].Is(item1);
				album.Map.Value.Items[0].Is(item1);
			}
		}

		[Test]
		public async Task FileSystemWatcher() {
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.TargetExtensions.Value = new[] { ".jpg" };
			using (var album = Get.Instance<AlbumForTest>()) {
				album.LoadFileInDirectoryArgs.Count.Is(0);
				album.Items.Count.Is(0);

				album.LoadFileInDirectoryArgs.Count.Is(0);

				// 存在するディレクトリならディレクトリ読み込みが発生して監視が始まる
				album.MonitoringDirectories.Add(TestDirectories["1"]);

				await Task.Delay(100);

				album.LoadFileInDirectoryArgs.Count.Is(1);
				album.LoadFileInDirectoryArgs[0].Is(TestDirectories["1"]);

				FileUtility.Copy(TestDirectories["0"], TestDirectories["1"], new[] {
					"image1.jpg",
					"image2.jpg"
				});

				await Task.Delay(100);

				album.Items.Count.Is(2);
				album.Items.Select(x => x.FileName.Value).Is(
					"image1.jpg",
					"image2.jpg");

				// 2回目もOK
				FileUtility.Copy(TestDirectories["0"], TestDirectories["1"], new[] {
					"image4.jpg",
					"image9.png"
				});

				await Task.Delay(100);

				album.Items.Count.Is(3);
				album.Items.Select(x => x.FileName.Value).Is(
					"image1.jpg",
					"image2.jpg",
					"image4.jpg");

				// サブディレクトリもOK
				FileUtility.Copy(TestDirectories["0"], TestDirectories["sub"], new[] {
					"image8.jpg"
				});

				await Task.Delay(100);

				album.Items.Count.Is(4);
				album.Items.Select(x => x.FileName.Value).Is(
					"image1.jpg",
					"image2.jpg",
					"image4.jpg",
					"image8.jpg");

				// 監視から外すとファイル追加されない
				album.MonitoringDirectories.Remove(TestDirectories["1"]);

				await Task.Delay(100);

				FileUtility.Copy(TestDirectories["0"], TestDirectories["1"], new[] {
					"image5.jpg"
				});

				await Task.Delay(500);

				album.Items.Count.Is(4);
				album.Items.Select(x => x.FileName.Value).Is(
					"image1.jpg",
					"image2.jpg",
					"image4.jpg",
					"image8.jpg");
			}
		}

		[Test]
		public async Task CurrentMediaFile() {

			var settings = Get.Instance<ISettings>();
			using (var album = Get.Instance<AlbumForTest>()) {
				var image1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
				var image2 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image2.jpg"));
				var image3 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image3.jpg"));

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Detail;
				image1.Image.Value.IsNull();
				image2.Image.Value.IsNull();
				image3.Image.Value.IsNull();

				image1.Exif.Value.IsNull();
				album.CurrentMediaFile.Value = image1;

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => image1.Image.Value != null)
					.Timeout(TimeSpan.FromSeconds(1))
					.FirstAsync();

				image1.Image.Value.IsNotNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image1);
				image1.Exif.Value.IsNotNull();

				album.CurrentMediaFile.Value = image2;

				await Observable.Interval(TimeSpan.FromSeconds(0.1))
					.Where(_ => image2.Image.Value != null)
					.Timeout(TimeSpan.FromSeconds(1))
					.FirstAsync();

				image1.Image.Value.IsNull();
				image2.Image.Value.IsNotNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image2);

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Library;

				album.CurrentMediaFile.Value = image3;

				await Task.Delay(1000);

				image2.Image.Value.IsNull();
				image3.Image.Value.IsNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image3);

				settings.GeneralSettings.DisplayMode.Value = DisplayMode.Map;

				album.CurrentMediaFile.Value = image1;

				await Task.Delay(1000);

				image3.Image.Value.IsNull();
				image1.Image.Value.IsNull();
				album.Map.Value.CurrentMediaFile.Value.Is(image1);
			}
		}

		[Test]
		public void CurrentMediaFiles() {
			using (var album = Get.Instance<AlbumForTest>()) {
				album.CurrentMediaFiles.Count.Is(0);
				album.MediaFileProperties.Value.Items.Count.Is(0);

				var item1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
				album.CurrentMediaFiles.Add(item1);

				album.CurrentMediaFiles.Count.Is(1);
				album.MediaFileProperties.Value.Items.Count.Is(1);
				album.CurrentMediaFiles[0].Is(item1);
				album.MediaFileProperties.Value.Items[0].Is(item1);
			}
		}

		[TestCase(DisplayMode.Detail)]
		[TestCase(DisplayMode.Library)]
		[TestCase(DisplayMode.Map)]
		public void ChangeDisplayMode(DisplayMode mode) {
			using (var album = Get.Instance<AlbumForTest>()) {
				var settings = Get.Instance<ISettings>();
				album.ChangeDisplayMode(mode);
				album.DisplayMode.Value.Is(mode);
				settings.GeneralSettings.DisplayMode.Value.Is(mode);
			}
		}

		private class AlbumForTest : MediaBox.Models.Album.Album {
			public readonly List<string> LoadFileInDirectoryArgs = new List<string>();

			public readonly List<MediaFile> OnAddedItemArgs = new List<MediaFile>();

			protected override Task LoadFileInDirectory(string directoryPath, CancellationToken cancellationToken) {
				this.LoadFileInDirectoryArgs.Add(directoryPath);
				return Task.FromResult<object>(null);
			}

			protected override void OnAddedItem(MediaFile mediaFile) {
				this.OnAddedItemArgs.Add(mediaFile);
			}
		}
	}
}
