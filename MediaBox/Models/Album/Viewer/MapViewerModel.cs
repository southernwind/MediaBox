
using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class MapViewerModel : ModelBase {
		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapModel> Map {
			get;
		}


		public MapViewerModel(IAlbumModel albumModel) {
			this.Map = new ReactivePropertySlim<MapModel>(new MapModel(albumModel.Items, albumModel.CurrentMediaFiles));
		}
	}
}
