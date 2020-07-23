using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	public class MapViewerViewModel : ViewModelBase, IAlbumViewerViewModel {
		public IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		public IAlbumViewModel AlbumViewModel {
			get;
		}

		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}


		public MapViewerViewModel(IAlbumViewModel albumViewModel, ISettings settings, ViewModelFactory viewModelFactory, IMapControl mapControl) {
			this.AlbumViewModel = albumViewModel;
			var model = new MapViewerModel(this.AlbumViewModel.AlbumModel, settings, mapControl);

			this.Map = model.Map.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}
}
