
using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Tests.Models.Map {
	internal class MapModelTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
		}

		[Test]
		public void MapApiKey() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var map = new MapModel(osc);
			this.Settings.GeneralSettings.BingMapApiKey.Value = "abcdefghijk";
			map.BingMapApiKey.Value.Is("abcdefghijk");
		}

		[Test]
		public void MapPinSize() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var map = new MapModel(osc);
			this.Settings.GeneralSettings.MapPinSize.Value = 193;
			map.MapPinSize.Value.Is(193);
		}

		[Test]
		public void カレントアイテム変更によるセンター座標変化() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var map = new MapModel(osc);

			var mf = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			mf.CreateDataBaseRecord();
			var noExif = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			noExif.CreateDataBaseRecord();

			map.CurrentMediaFile.Value = noExif;

			map.CenterLocation.Value.Latitude.Is(0);
			map.CenterLocation.Value.Longitude.Is(0);
			map.ZoomLevel.Value.Is(0);

			map.CurrentMediaFile.Value = null;
			map.CenterLocation.Value.Latitude.Is(0);
			map.CenterLocation.Value.Longitude.Is(0);
			map.ZoomLevel.Value.Is(0);

			map.CurrentMediaFile.Value = mf;
			Assert.AreEqual(map.CenterLocation.Value.Latitude, 34.697419, 0.001);
			Assert.AreEqual(map.CenterLocation.Value.Longitude, 135.533553, 0.001);
			map.ZoomLevel.Value.Is(14);

			map.CurrentMediaFile.Value = null;
			Assert.AreEqual(map.CenterLocation.Value.Latitude, 34.697419, 0.001);
			Assert.AreEqual(map.CenterLocation.Value.Longitude, 135.533553, 0.001);
			map.ZoomLevel.Value.Is(14);

			map.CurrentMediaFile.Value = noExif;
			Assert.AreEqual(map.CenterLocation.Value.Latitude, 34.697419, 0.001);
			Assert.AreEqual(map.CenterLocation.Value.Longitude, 135.533553, 0.001);
			map.ZoomLevel.Value.Is(14);
		}

		[Test]
		public void アイテムリスト変更によるセンター座標変化() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var map = new MapModel(osc);

			var mf = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			mf.CreateDataBaseRecord();
			var noExif = this.MediaFactory.Create(this.TestFiles.NoExifJpg.FilePath);
			noExif.CreateDataBaseRecord();
			var png = this.MediaFactory.Create(this.TestFiles.Image4Png.FilePath);
			png.CreateDataBaseRecord();
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			image2.CreateDataBaseRecord();

			map.CurrentMediaFile.Value = null;

			map.CenterLocation.Value.Latitude.Is(0);
			map.CenterLocation.Value.Longitude.Is(0);
			map.ZoomLevel.Value.Is(0);

			map.Items.Add(png);
			map.Items.Add(mf);
			map.Items.Add(noExif);

			Assert.AreEqual(map.CenterLocation.Value.Latitude, 34.697419, 0.001);
			Assert.AreEqual(map.CenterLocation.Value.Longitude, 135.533553, 0.001);
			map.ZoomLevel.Value.Is(14);

			// それでもカレントアイテムがあればカレントアイテム優先
			map.CurrentMediaFile.Value = image2;
			Assert.AreEqual(map.CenterLocation.Value.Latitude, -35.184364, 0.001);
			Assert.AreEqual(map.CenterLocation.Value.Longitude, -132.183486, 0.001);
			map.ZoomLevel.Value.Is(14);
		}
	}
}
