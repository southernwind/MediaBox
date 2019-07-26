using System.Collections.Generic;
using System.Linq;

using Livet;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
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
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);

			vm.MapControl.Value.Is(model.MapControl.Value);
		}

		[Test]
		public void マップ用アイテムグループリスト() {
			using var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);

			model.ItemsForMapView.Value = new[]{
				new MapPin(image1, default),
				new MapPin(image2, default),
				new MapPin(image3, default)
			};

			vm.ItemsForMapView.Value.Count().Is(3);
			vm.ItemsForMapView.Value.Select(x => x.Model).Is(model.ItemsForMapView.Value);
		}

		[Test]
		public void ポインター() {
			using var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);


			var group = new MapPin(image1, default(Rectangle));
			model.Pointer.Value = group;

			vm.Pointer.Value.Model.Is(group);
		}

		[Test]
		public void ポインターgps座標() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);


			model.PointerLocation.Value = new GpsLocation(50.3, 81.53);
			vm.PointerLocation.Value.Latitude.Is(50.3);
			vm.PointerLocation.Value.Longitude.Is(81.53);
		}

		[Test]
		public void マップapiキー() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);


			this.Settings.GeneralSettings.BingMapApiKey.Value = "asdfghjkl";
			vm.BingMapApiKey.Value.Is("asdfghjkl");
		}

		[Test]
		public void ピンサイズ() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);

			this.Settings.GeneralSettings.MapPinSize.Value = 55;
			vm.MapPinSize.Value.Is(55);
		}

		[Test]
		public void ズームレベル() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);

			model.ZoomLevel.Value = 18;
			vm.ZoomLevel.Value.Is(18);

			vm.ZoomLevel.Value = 13;
			model.ZoomLevel.Value.Is(13);
		}

		[Test]
		public void ピン選択() {
			using var image1 = this.MediaFactory.Create(this.TestFiles.Image1Jpg.FilePath);
			using var image2 = this.MediaFactory.Create(this.TestFiles.Image2Jpg.FilePath);
			using var image3 = this.MediaFactory.Create(this.TestFiles.Image3Jpg.FilePath);
			using var pin1 = new MapPin(image1, default);
			using var pin2 = new MapPin(image2, default);
			pin2.Items.Add(image3);
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var model = new MapModel(osc, rp);
			using var vm = new MapViewModel(model);
			model.ItemsForMapView.Value = new[] { pin1, pin2 };
			pin1.PinState.Value.Is(PinState.Unselected);
			pin2.PinState.Value.Is(PinState.Unselected);
			vm.SelectCommand.Execute(this.ViewModelFactory.Create(pin2));
			pin1.PinState.Value.Is(PinState.Unselected);
			pin2.PinState.Value.Is(PinState.Selected);
		}
	}
}
