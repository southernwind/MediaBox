using System.IO;
using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture]
	internal class GpsSelectorTest : ModelTestClassBase {
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
			image1.CreateDataBaseRecord();
			image2.CreateDataBaseRecord();
			image3.CreateDataBaseRecord();

			db.MediaFiles
				.ToList()
				.Select(x => (x.Latitude, x.Longitude))
				.OrderBy(x => x)
				.Is(
					(35.371769444444446d, 136.81027222222224d),
					(35.373641666666664d, 136.81321666666668d),
					(35.651713888888885d, 136.82127499999999d)
				);

			gs.TargetFiles.Value.Count().Is(2);
			gs.Location.Value = new GpsLocation(40, 70);
			gs.SetGps();
			image1.Location.Latitude.Is(40);
			image2.Location.Latitude.Is(40);
			image3.Location.Is(new GpsLocation(35.371769444444446d, 136.81027222222224d));
			image1.Location.Longitude.Is(70);
			image2.Location.Longitude.Is(70);
			image3.Location.Is(new GpsLocation(35.371769444444446d, 136.81027222222224d));
			gs.TargetFiles.Value.Count().Is(0);

			db.MediaFiles
				.ToList()
				.Select(x => (x.Latitude, x.Longitude))
				.Is((40, 70), (40, 70), (35.371769444444446d, 136.81027222222224d));
		}
	}
}
