using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.Album.Objects;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album.Viewer;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.ViewModels.Album.Viewer {
	public class MapViewerViewModel : AlbumViewerViewModel {
		public override IReactiveProperty<bool> IsSelected {
			get;
		} = new ReactivePropertySlim<bool>();

		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}

		public MapViewerViewModel(IAlbumViewModel albumViewModel, ISettings settings, ViewModelFactory viewModelFactory, IMapControl mapControl, IMediaFileListContextMenuViewModel contextMenuViewModel) : base(albumViewModel, contextMenuViewModel) {
			var model = new MapViewerModel(this.AlbumViewModel.AlbumModel, settings, mapControl);

			this.Map = model.Map.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
		}
	}
}
