
using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Map;
namespace SandBeige.MediaBox.Models.Album.Viewer {
	public class MapViewerModel : ModelBase {
		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapModel> Map {
			get;
		}


		public MapViewerModel(IAlbumModel albumModel, ISettings settings, IMapControl mapControl) {
			this.Map = new ReactivePropertySlim<MapModel>(new MapModel(albumModel.Items, albumModel.CurrentMediaFiles, settings, mapControl));
		}
	}
}
