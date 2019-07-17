
using System.Collections.Generic;

using Livet;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Tests.Models.Map {
	internal class MapModelTest : ModelTestClassBase {
		[Test]
		public void MapApiKey() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			this.Settings.GeneralSettings.BingMapApiKey.Value = "abcdefghijk";
			map.BingMapApiKey.Value.Is("abcdefghijk");
		}

		[Test]
		public void MapPinSize() {
			var osc = new ObservableSynchronizedCollection<IMediaFileModel>();
			var rp = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>();
			using var map = new MapModel(osc, rp);
			this.Settings.GeneralSettings.MapPinSize.Value = 193;
			map.MapPinSize.Value.Is(193);
		}
	}
}
