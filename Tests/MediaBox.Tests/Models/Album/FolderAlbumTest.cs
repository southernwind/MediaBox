﻿using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Tests.TestUtility;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class FolderAlbumTest : TestClassBase {
		[Test]
		public async Task LoadFileInDirectory() {
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.TargetExtensions.Value = new[] { ".jpg" };

			FileUtility.Copy(
				TestDataDir,
				TestDirectories["1"],
				new[] { "image1.jpg", "image2.jpg", "image4.jpg", "image6.jpg", "image8.jpg", "image9.png" });


			FileUtility.Copy(
				TestDataDir,
				TestDirectories["sub"],
				new[] { "image3.jpg", "image7.jpg" });


			FileUtility.Copy(
				TestDataDir,
				TestDirectories["2"],
				new[] { "image5.jpg" });

			using (var album1 = Get.Instance<FolderAlbum>(TestDirectories["1"])) {
				await album1.ProcessingMonitoringDirectory();

				album1.Items.Count.Is(7);
				album1.Items.Select(x => x.FilePath).Is(
					Path.Combine(TestDirectories["1"], "image1.jpg"),
					Path.Combine(TestDirectories["1"], "image2.jpg"),
					Path.Combine(TestDirectories["1"], "image4.jpg"),
					Path.Combine(TestDirectories["1"], "image6.jpg"),
					Path.Combine(TestDirectories["1"], "image8.jpg"),
					Path.Combine(TestDirectories["sub"], "image3.jpg"),
					Path.Combine(TestDirectories["sub"], "image7.jpg")
				);

				// 2回目実行しても同じファイルは追加されない
				album1.MonitoringDirectories.Add(TestDirectories["1"]);

				await album1.ProcessingMonitoringDirectory();

				album1.Items.Count.Is(7);

				// 存在しないフォルダの場合は何も起こらない
				album1.MonitoringDirectories.Add($"{TestDirectories["1"]}____");

				await album1.ProcessingMonitoringDirectory();

				album1.Items.Count.Is(7);
			}
		}

		[Test]
		public void FolderAlbum() {
			using (var album = Get.Instance<FolderAlbum>(TestDirectories["0"])) {
				album.MonitoringDirectories.Count.Is(1);
				album.MonitoringDirectories.Is(
					TestDirectories["0"]);
				album.Title.Value.Is(TestDirectories["0"]);
			}
		}

		[Test]
		public async Task OnAddedItem() {
			var thumbDir = Get.Instance<ISettings>().PathSettings.ThumbnailDirectoryPath.Value;
			using (var album1 = Get.Instance<FolderAlbum>(TestDirectories["1"])) {

				await album1.ProcessingMonitoringDirectory();

				album1.Count.Value.Is(0);
				Directory.GetFiles(thumbDir).Length.Is(0);
				FileUtility.Copy(TestDataDir, TestDirectories["1"], new[] { "image1.jpg" });

				await Task.Delay(100);

				// こっちはやむなし
				await Observable
					.Interval(TimeSpan.FromMilliseconds(100))
					.Where(x =>
						album1.Count.Value != 0 &&
						album1.Items.First().Thumbnail != null &&
						album1.Items.First().Exif != null)
					.Timeout(TimeSpan.FromSeconds(2))
					.FirstAsync();

				album1.Count.Value.Is(1);

				var media1 = album1.Items.First();
				media1.MediaFileId.IsNull();
				media1.Exif.IsNotNull();
				media1.Thumbnail.IsNotNull();
				Directory.GetFiles(thumbDir).Length.Is(0);
				Assert.AreEqual(35.6517139, media1.Latitude, 0.00001);
				Assert.AreEqual(136.821275, media1.Longitude, 0.00001);
				media1.Orientation.Is(1);
			}
		}
	}
}
