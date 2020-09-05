using System;
using System.Collections.Generic;
using System.Reactive;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Map {
	public interface IMapModel : IMediaFileCollection {
		IReadOnlyReactiveProperty<string?> BingMapApiKey {
			get;
		}
		IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		}
		IReactiveProperty<IEnumerable<IMediaFileModel>> IgnoreMediaFiles {
			get;
		}
		IReactiveProperty<IEnumerable<IMapPin>> ItemsForMapView {
			get;
		}
		IReactiveProperty<IMapControl> MapControl {
			get;
		}
		IReadOnlyReactiveProperty<int> MapPinSize {
			get;
		}
		IObservable<Unit> OnDecide {
			get;
		}
		IObservable<GpsLocation> OnMove {
			get;
		}
		IReactiveProperty<IMapPointer?> Pointer {
			get;
		}
		IReactiveProperty<GpsLocation> PointerLocation {
			get;
		}
		IReactiveProperty<double> ZoomLevel {
			get;
		}

		void Select(IMapPin mediaGroup);
		string ToString();
	}
}