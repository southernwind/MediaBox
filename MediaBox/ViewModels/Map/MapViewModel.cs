using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Map {
	internal class MapViewModel : ViewModelBase {
		private readonly MapModel _model;

		/// <summary>
		/// マップコントロール
		/// </summary>
		public ReadOnlyReactivePropertySlim<IMapControl> MapControl {
			get;
		}

		/// <summary>
		/// マップ用アイテムグループリスト
		/// </summary>
		public ReadOnlyReactiveCollection<MediaGroupViewModel> ItemsForMapView {
			get;
		}

		/// <summary>
		/// ポインター
		/// </summary>
		public ReadOnlyReactivePropertySlim<MediaGroupViewModel> Pointer {
			get;
		}

		/// <summary>
		/// ポインター緯度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double> PointerLatitude {
			get;
		}

		/// <summary>
		/// ポインター経度
		/// </summary>
		public ReadOnlyReactivePropertySlim<double> PointerLongitude {
			get;
		}

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> BingMapApiKey {
			get;
		}

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public ReadOnlyReactivePropertySlim<int> MapPinSize {
			get;
		}

		/// <summary>
		/// 拡大
		/// </summary>
		public ReactiveProperty<double> ZoomLevel {
			get;
		}

		/// <summary>
		/// 中心座標　緯度
		/// </summary>
		public ReactiveProperty<double> CenterLatitude {
			get;
		}

		/// <summary>
		/// 中心座標 経度
		/// </summary>
		public ReactiveProperty<double> CenterLongitude {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public MapViewModel(MapModel model) {
			this._model = model;
			this.MapControl = this._model.MapControl.ToReadOnlyReactivePropertySlim();
			this.ItemsForMapView = this._model.ItemsForMapView.ToReadOnlyReactiveCollection(x => Get.Instance<MediaGroupViewModel>(x));
			this.Pointer = this._model.Pointer.Select(x => x == null ? default : Get.Instance<MediaGroupViewModel>(x)).ToReadOnlyReactivePropertySlim();
			this.PointerLatitude = this._model.PointerLatitude.ToReadOnlyReactivePropertySlim();
			this.PointerLongitude = this._model.PointerLongitude.ToReadOnlyReactivePropertySlim();
			this.BingMapApiKey = this._model.BingMapApiKey.ToReadOnlyReactivePropertySlim();
			this.MapPinSize = this._model.MapPinSize.ToReadOnlyReactivePropertySlim();
			this.ZoomLevel = this._model.ZoomLevel.ToReactivePropertyAsSynchronized(x => x.Value);
			this.CenterLatitude = this._model.CenterLatitude.ToReactivePropertyAsSynchronized(x => x.Value);
			this.CenterLongitude = this._model.CenterLongitude.ToReactivePropertyAsSynchronized(x => x.Value);
		}
	}
}
