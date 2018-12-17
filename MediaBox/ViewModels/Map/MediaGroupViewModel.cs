using System.Reactive.Linq;
using Reactive.Bindings;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Map {
	internal class MediaGroupViewModel : MediaFileCollectionViewModel<MediaGroup> {
		private readonly MediaGroup _model;
		public ReadOnlyReactivePropertySlim<MediaFileViewModel> Core {
			get;
		}

		public MediaGroupViewModel(MediaGroup model) : base(model) {
			this._model = model;

			this.Core = this._model.Core.Select(x => Get.Instance<MediaFileViewModel>(x)).ToReadOnlyReactivePropertySlim();
		}
	}
}
