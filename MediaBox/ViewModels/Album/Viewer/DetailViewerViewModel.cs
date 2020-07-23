
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album.Viewer;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	public class DetailViewerViewModel : ViewModelBase, IAlbumViewerViewModel {
		public IReactiveProperty<bool> IsSelected {
			get;
		}

		public IAlbumViewModel AlbumViewModel {
			get;
		}

		public DetailViewerViewModel(IAlbumViewModel albumViewModel) {
			this.AlbumViewModel = albumViewModel;
			var model = new DetailViewerModel(this.AlbumViewModel.AlbumModel);

			this.IsSelected = model.IsVisible.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
		}
	}
}
