using System.Linq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.TestUtilities;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture]
	internal class GpsSelectorTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

		[Test]
		public void 候補ファイルリスト() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			image1.CreateDataBaseRecord();
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			image2.CreateDataBaseRecord();
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			image3.CreateDataBaseRecord();
			var gs = Get.Instance<GpsSelector>();
			gs.CandidateMediaFiles.Add(image1);
			gs.CandidateMediaFiles.Add(image2);
			gs.CandidateMediaFiles.Add(image3);
			gs.CandidateMediaFiles.Count.Is(3);
			gs.CandidateMediaFiles.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);
			gs.Map.Value.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);
		}

		[Test]
		public void 対象ファイル() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			image1.CreateDataBaseRecord();
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			image2.CreateDataBaseRecord();
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			image3.CreateDataBaseRecord();
			var gs = Get.Instance<GpsSelector>();

			gs.TargetFiles.Value = new[]{
				image2,
				image3
			};
			gs.TargetFiles.Value.Check(this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);
			gs.Map.Value.IgnoreMediaFiles.Value.Check(this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);

			gs.Map.Value.Pointer.Value.Core.Value.Is(image2);
			gs.Map.Value.Pointer.Value.Items.Check(this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);
			gs.Map.Value.Pointer.Value.Count.Value.Is(2);

			gs.TargetFiles.Value = new[]{
				image1,
				image2,
				image3
			};
			gs.TargetFiles.Value.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);
			gs.Map.Value.IgnoreMediaFiles.Value.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);

			gs.Map.Value.Pointer.Value.Core.Value.Is(image1);
			gs.Map.Value.Pointer.Value.Items.Check(this.TestFiles.Image1Jpg, this.TestFiles.Image2Jpg, this.TestFiles.Image3Jpg);
			gs.Map.Value.Pointer.Value.Count.Value.Is(3);
		}


		[Test]
		public void GPS再設定() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var r1 = image1.CreateDataBaseRecord();
			var r2 = image2.CreateDataBaseRecord();
			var r3 = image3.CreateDataBaseRecord();
			this.DataBase.MediaFiles.AddRange(r1, r2, r3);
			this.DataBase.SaveChanges();
			image1.MediaFileId = r1.MediaFileId;
			image2.MediaFileId = r2.MediaFileId;
			image3.MediaFileId = r3.MediaFileId;


			var gs = Get.Instance<GpsSelector>();
			gs.TargetFiles.Value = new[] {
				image1,
				image3
			};

			this.DataBase.MediaFiles
				.OrderBy(x => x.MediaFileId)
				.ToList()
				.Select(x => (x.Latitude, x.Longitude))
				.Is(
					(34.697419444444442d, 135.53355277777777d),
					(-35.184363888888889d, -132.1834861111111d),
					(null, null)
				);

			gs.TargetFiles.Value.Count().Is(2);
			gs.Location.Value = new GpsLocation(40, 70);
			gs.SetGps();
			image1.Location.Is(new GpsLocation(40, 70));
			image2.Location.Latitude.Is(-35.184363888888889d);
			image2.Location.Longitude.Is(-132.1834861111111d);
			image3.Location.Is(new GpsLocation(40, 70));

			this.DataBase.MediaFiles
				.OrderBy(x => x.MediaFileId)
				.ToList()
				.Select(x => (x.Latitude, x.Longitude))
				.Is(
					(40, 70),
					(-35.184363888888889d, -132.1834861111111d),
					(40, 70)
				);

			gs.TargetFiles.Value.Count().Is(0);
		}
	}
}
