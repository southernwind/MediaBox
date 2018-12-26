using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture]
	internal class GpsSelectorTest : TestClassBase {
		[Test]
		public void CandidateMediaFiles() {
			var image1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image3.jpg"));
			var gs = Get.Instance<GpsSelector>();
			gs.CandidateMediaFiles.Add(image1);
			gs.CandidateMediaFiles.Add(image2);
			gs.CandidateMediaFiles.Add(image3);
			gs.CandidateMediaFiles.Count.Is(3);
			gs.CandidateMediaFiles.OrderBy(x => x.FileName.Value).Is(gs.Map.Value.Items.OrderBy(x => x.FileName.Value));
		}

		[Test]
		public void TargetFiles() {
			var image1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image3.jpg"));
			var gs = Get.Instance<GpsSelector>();

			gs.TargetFiles.Add(image1);
			gs.TargetFiles.Add(image2);
			gs.TargetFiles.Add(image3);
			gs.TargetFiles.Count.Is(3);

			gs.Map.Value.Pointer.Value.Core.Value.Is(image1);
			gs.Map.Value.Pointer.Value.Count.Value.Is(3);
			gs.TargetFiles.Is(gs.Map.Value.IgnoreMediaFiles);
			gs.Map.Value.IgnoreMediaFiles.Count.Is(3);
		}

		[Test]
		public void Latitude() {
			var gs = Get.Instance<GpsSelector>();
			gs.Latitude.Value = 15;
			gs.Map.Value.PointerLatitude.Value.Is(15);
			gs.Latitude.Value = 70;
			gs.Map.Value.PointerLatitude.Value.Is(70);
			gs.Map.Value.PointerLongitude.Value.Is(0);
			gs.Map.Value.CenterLatitude.Value.Is(0);
			gs.Map.Value.CenterLongitude.Value.Is(0);
		}

		[Test]
		public void Longitude() {
			var gs = Get.Instance<GpsSelector>();
			gs.Longitude.Value = 15;
			gs.Map.Value.PointerLongitude.Value.Is(15);
			gs.Longitude.Value = 70;
			gs.Map.Value.PointerLongitude.Value.Is(70);
			gs.Map.Value.PointerLatitude.Value.Is(0);
			gs.Map.Value.CenterLatitude.Value.Is(0);
			gs.Map.Value.CenterLongitude.Value.Is(0);
		}


		[Test]
		public async Task SetGps() {
			var image1 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDirectories["0"], "image3.jpg"));
			var gs = Get.Instance<GpsSelector>();
			gs.TargetFiles.Add(image1);
			gs.TargetFiles.Add(image2);

			using (var album = Get.Instance<RegisteredAlbumForTest>()) {
				album.Create();
				await album.CallOnAddedItemAsync(image1);
				await album.CallOnAddedItemAsync(image2);
				await album.CallOnAddedItemAsync(image3);
			}

			gs.TargetFiles.Count.Is(2);
			gs.Latitude.Value = 40;
			gs.Longitude.Value = 70;
			gs.SetGps();
			gs.TargetFiles.Count.Is(0);
			image1.Latitude.Value.Is(40);
			image2.Latitude.Value.Is(40);
			image3.Latitude.Value.IsNot(40);
			image1.Longitude.Value.Is(70);
			image2.Longitude.Value.Is(70);
			image3.Longitude.Value.IsNot(70);
		}

		/// <summary>
		/// protectedメソッドを呼び出すためのテスト用クラス
		/// </summary>
		private class RegisteredAlbumForTest : RegisteredAlbum {
			public void CallLoadFileInDirectory(string directoryPath) {
				this.LoadFileInDirectory(directoryPath);
			}

			public async Task CallOnAddedItemAsync(MediaFile mediaFile) {
				await this.OnAddedItemAsync(mediaFile);
			}
		}
	}
}
