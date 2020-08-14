using System.Collections.Generic;

using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models;
using SandBeige.MediaBox.Composition.Interfaces.Models.Gesture;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Models.Map {
	public interface IGpsSelector : IModelBase {
		ObservableSynchronizedCollection<IMediaFileModel> CandidateMediaFiles {
			get;
		}
		IGestureReceiver GestureReceiver {
			get;
		}
		IReactiveProperty<GpsLocation> Location {
			get;
		}
		IReactiveProperty<IMapModel> Map {
			get;
		}
		IReactiveProperty<IEnumerable<IMediaFileModel>> TargetFiles {
			get;
		}
		IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		void SetGps();
		string ToString();
	}
}