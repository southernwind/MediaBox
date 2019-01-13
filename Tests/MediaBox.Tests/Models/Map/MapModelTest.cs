﻿using System.IO;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Tests.Models.Map {
	[TestFixture]
	internal class MapModelTest : TestClassBase {
		[Test]
		public void BingMapApiKey() {
			var map = Get.Instance<MapModel>();
			var settings = Get.Instance<ISettings>();
			map.BingMapApiKey.Value.IsNull();
			settings.GeneralSettings.BingMapApiKey.Value = "abcdefghijk";
			map.BingMapApiKey.Value.Is("abcdefghijk");
		}

		[Test]
		public void MapPinSize() {
			var map = Get.Instance<MapModel>();
			var settings = Get.Instance<ISettings>();
			settings.GeneralSettings.MapPinSize.Value = 193;
			map.MapPinSize.Value.Is(193);
		}

		[Test]
		public void ZoomLevel() {
			var map = Get.Instance<MapModel>();
			map.ZoomLevel.Value.Is(0);

			var mf = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			map.CurrentMediaFile.Value = mf;
			map.ZoomLevel.Value.Is(0);

			map.CurrentMediaFile.Value = null;
			map.ZoomLevel.Value.Is(0);

			mf.Latitude = 135;
			mf.Longitude = 45;
			map.CurrentMediaFile.Value = mf;

			map.ZoomLevel.Value.Is(14);

			map.CurrentMediaFile.Value = null;
			map.ZoomLevel.Value.Is(0);
		}

		[Test]
		public void CenterLocation() {
			var map = Get.Instance<MapModel>();
			map.ZoomLevel.Value.Is(0);

			var mf = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			map.CurrentMediaFile.Value = mf;
			map.CenterLatitude.Value.Is(0);
			map.CenterLongitude.Value.Is(0);

			map.CurrentMediaFile.Value = null;
			map.CenterLatitude.Value.Is(0);
			map.CenterLongitude.Value.Is(0);

			mf.Latitude = 135;
			mf.Longitude = 45;
			map.CurrentMediaFile.Value = mf;

			map.CenterLatitude.Value.Is(135);
			map.CenterLongitude.Value.Is(45);

			map.CurrentMediaFile.Value = null;
			map.CenterLatitude.Value.Is(0);
			map.CenterLongitude.Value.Is(0);

			map.Items.Add(mf);
			map.CenterLatitude.Value.Is(135);
			map.CenterLongitude.Value.Is(45);
		}


	}
}
