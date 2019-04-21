using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase.Tables;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Album {
	[TestFixture]
	internal class AlbumTest : ModelTestClassBase {
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
				album.MediaFileInformation.Value.Files.Value.Count().Is(0);

				var item1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
				album.CurrentMediaFiles.Value = new[] { item1 };

				album.CurrentMediaFiles.Value.Count().Is(1);
				album.MediaFileInformation.Value.Files.Value.Count().Is(1);
				album.CurrentMediaFiles.Value.First().Is(item1);
				album.MediaFileInformation.Value.Files.Value.First().Is(item1);
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

			public AlbumForTest() : base(new ObservableSynchronizedCollection<IMediaFileModel>()) {

			}

			protected override Expression<Func<MediaFile, bool>> WherePredicate() {
				return _ => true;
			}
		}
	}
}
