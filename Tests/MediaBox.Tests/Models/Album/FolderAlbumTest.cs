﻿using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class FolderAlbumTest : TestClassBase {
		[Test]
		public void LoadFileInDirectory() {
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
				album1.Items.Count.Is(7);
				album1.Items.Select(x => x.FilePath.Value).Is(
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
				album1.Items.Count.Is(7);

				// 存在しないフォルダの場合は何も起こらない
				album1.MonitoringDirectories.Add($"{TestDirectories["1"]}____");
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
			using (var album1 = Get.Instance<FolderAlbum>(TestDirectories["1"])) {
				album1.Count.Value.Is(0);
				var thumbDir = Get.Instance<ISettings>().PathSettings.ThumbnailDirectoryPath.Value;
				FileUtility.Copy(TestDataDir, TestDirectories["1"], new[] { "image1.jpg" });

				// TODO : サムネイル作成のFileStream作成で例外発生 理由はわからない

				// こっちはやむなし
				await Observable
					.Interval(TimeSpan.FromMilliseconds(100))
					.Where(x =>
						album1.Count.Value != 0 &&
						album1.Items.First().Thumbnail.Value != null &&
						album1.Items.First().Exif.Value != null)
					.Timeout(TimeSpan.FromSeconds(2))
					.FirstAsync();

				album1.Count.Value.Is(1);

				var media1 = album1.Items.First();
				media1.MediaFileId.IsNull();
				media1.Exif.Value.IsNotNull();
				media1.Thumbnail.Value.IsNotNull();
				Directory.GetFiles(thumbDir).Length.Is(0);
				Assert.AreEqual(35.6517139, media1.Latitude.Value, 0.00001);
				Assert.AreEqual(136.821275, media1.Longitude.Value, 0.00001);
				media1.Orientation.Value.Is(1);
			}
		}
	}
}
