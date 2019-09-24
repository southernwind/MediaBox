
using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class MapViewerModel : ModelBase {

		/// <summary>
		/// 表示する列
		/// </summary>
		public ReadOnlyReactiveCollection<Col> Columns {
			get;
		}

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
