using System.IO;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
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
			gs.CandidateMediaFiles.OrderBy(x => x.FileName).Is(gs.Map.Value.Items.OrderBy(x => x.FileName));
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
		public void Location() {
			var gs = Get.Instance<GpsSelector>();
			gs.Location.Value = new GpsLocation(15, 33);
			gs.Map.Value.PointerLocation.Value.Latitude.Is(15);
			gs.Location.Value = new GpsLocation(70, 99);
			gs.Map.Value.PointerLocation.Value.Latitude.Is(70);
			gs.Map.Value.PointerLocation.Value.Longitude.Is(99);
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
			gs.Location.Value = new GpsLocation(40, 70);
			gs.SetGps();
			image1.Location.Latitude.Is(40);
			image2.Location.Latitude.Is(40);
			image3.Location.IsNull();
			image1.Location.Longitude.Is(70);
			image2.Location.Longitude.Is(70);
			image3.Location.IsNull();
			gs.TargetFiles.Value.Count().Is(0);

			db.MediaFiles
				.ToList()
				.Select(x => (x.Latitude, x.Longitude))
				.Is((40, 70), (40, 70), (null, null));
		}
	}
}
