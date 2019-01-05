using System.IO;
using System.Linq;
using System.Reactive.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture]
	internal class GpsSelectorTest : TestClassBase {
		[Test]
		public void CandidateMediaFiles() {
			var image1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
			var gs = Get.Instance<GpsSelector>();
			gs.CandidateMediaFiles.Add(image1);
			gs.CandidateMediaFiles.Add(image2);
			gs.CandidateMediaFiles.Add(image3);
			gs.CandidateMediaFiles.Count.Is(3);
			gs.CandidateMediaFiles.OrderBy(x => x.FileName.Value).Is(gs.Map.Value.Items.OrderBy(x => x.FileName.Value));
		}

		[Test]
		public void TargetFiles() {
			var image1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
			var gs = Get.Instance<GpsSelector>();

			gs.TargetFiles.Value = new[]{
				image1,
				image2,
				image3
			};
			gs.TargetFiles.Value.Count().Is(3);

			gs.Map.Value.Pointer.Value.Core.Value.Is(image1);
			gs.Map.Value.Pointer.Value.Count.Value.Is(3);
			gs.TargetFiles.Value.Is(gs.Map.Value.IgnoreMediaFiles.Value);
			gs.Map.Value.IgnoreMediaFiles.Value.Count().Is(3);
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
		public void SetGps() {
			var db = Get.Instance<MediaBoxDbContext>();
			var image1 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			var image2 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image2.jpg"));
			var image3 = this.MediaFactory.Create(Path.Combine(TestDataDir, "image3.jpg"));
			var gs = Get.Instance<GpsSelector>();
			gs.TargetFiles.Value = new[] {
				image1,
				image2
			};

			image1.RegisterToDataBase();
			image2.RegisterToDataBase();
			image3.RegisterToDataBase();

			db.MediaFiles
				.ToList()
				.Select(x => (x.Latitude, x.Longitude))
				.Is((null, null), (null, null), (null, null));

			gs.TargetFiles.Value.Count().Is(2);
			gs.Latitude.Value = 40;
			gs.Longitude.Value = 70;
			gs.SetGps();
			image1.Latitude.Value.Is(40);
			image2.Latitude.Value.Is(40);
			image3.Latitude.Value.IsNull();
			image1.Longitude.Value.Is(70);
			image2.Longitude.Value.Is(70);
			image3.Longitude.Value.IsNull();
			gs.TargetFiles.Value.Count().Is(0);

			db.MediaFiles
				.ToList()
				.Select(x => (x.Latitude, x.Longitude))
				.Is((40, 70), (40, 70), (null, null));
		}
	}
}
