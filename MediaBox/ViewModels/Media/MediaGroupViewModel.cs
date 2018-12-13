using SandBeige.MediaBox.Library.Map;

namespace SandBeige.MediaBox.ViewModels.Media {
	internal class MediaGroupViewModel : MediaFileCollectionViewModel {
		public MediaFileViewModel Core {
			get;
		}

		public Rectangle CoreRectangle {
			get;
		}

		public MediaGroupViewModel(MediaFileViewModel core, Rectangle rectangle) {
			this.Core = core;
			this.Add(core);
			this.CoreRectangle = rectangle;
		}
	}
}
