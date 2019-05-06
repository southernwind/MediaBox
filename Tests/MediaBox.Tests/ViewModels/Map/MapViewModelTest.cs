using System;
using System.Collections.Generic;
using System.Linq;
using Livet;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.Tests.ViewModels.Map {
	[TestFixture]
	internal class MapViewModelTest : ViewModelTestClassBase {
		[Test]
		public void マップコントロール() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);

			vm.MapControl.Value.Is(model.MapControl.Value);
		}

		[Test]
		public void マップ用アイテムグループリスト() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);

			model.ItemsForMapView.Add(new MapPin(image1, default));
			model.ItemsForMapView.Add(new MapPin(image2, default));
			model.ItemsForMapView.Add(new MapPin(image3, default));

			vm.ItemsForMapView.Count.Is(3);
			vm.ItemsForMapView.Select(x => x.Model).Is(model.ItemsForMapView);
		}

		[Test]
		public void ポインター() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);


			var group = Get.Instance<MapPin>(image1, default(Rectangle));
			model.Pointer.Value = group;

			vm.Pointer.Value.Model.Is(group);
		}

		[Test]
		public void ポインターgps座標() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);


			model.PointerLocation.Value = new GpsLocation(50.3, 81.53);
			vm.PointerLocation.Value.Latitude.Is(50.3);
			vm.PointerLocation.Value.Longitude.Is(81.53);
		}

		[Test]
		public void マップapiキー() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);


			this.Settings.GeneralSettings.BingMapApiKey.Value = "asdfghjkl";
			vm.BingMapApiKey.Value.Is("asdfghjkl");
		}

		[Test]
		public void ピンサイズ() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);

			this.Settings.GeneralSettings.MapPinSize.Value = 55;
			vm.MapPinSize.Value.Is(55);
		}

		[Test]
		public void ズームレベル() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);

			model.ZoomLevel.Value = 18;
			vm.ZoomLevel.Value.Is(18);

			vm.ZoomLevel.Value = 13;
			model.ZoomLevel.Value.Is(13);
		}

		[Test]
		public void ピン選択() {
			var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var pin1 = new MapPin(image1, default);
			var pin2 = new MapPin(image2, default);
			pin2.Items.Add(image3);
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var model = new MapModel(osc);
			var vm = new MapViewModel(model);
			model.ItemsForMapView.AddRange(pin1, pin2);
			IEnumerable<IMediaFileModel> result = null;
			model.OnSelect.Subscribe(x => {
				result = x;
			});
			vm.SelectCommand.Execute(this.ViewModelFactory.Create(pin2));
			result.Is(image2, image3);
		}
	}
}
