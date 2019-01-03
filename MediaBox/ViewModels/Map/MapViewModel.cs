using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.ViewModels.Map {
	internal class MapViewModel : ViewModelBase {
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
		/// マップ上のピン選択コマンド
		/// </summary>
		public ReactiveCommand<MediaGroupViewModel> SelectCommand {
			get;
		} = new ReactiveCommand<MediaGroupViewModel>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public MapViewModel(MapModel model) {
			this.MapControl = model.MapControl.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.ItemsForMapView =
				model
					.ItemsForMapView
					.ToReadOnlyReactiveCollection(this.ViewModelFactory.Create, disposeElement: false)
					.AddTo(this.CompositeDisposable);
			this.Pointer =
				model.Pointer
					.Select(x => x == null ? default : this.ViewModelFactory.Create(x))
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);
			this.PointerLatitude = model.PointerLatitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.PointerLongitude = model.PointerLongitude.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.BingMapApiKey = model.BingMapApiKey.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.MapPinSize = model.MapPinSize.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.ZoomLevel = model.ZoomLevel.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.CenterLatitude = model.CenterLatitude.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.CenterLongitude = model.CenterLongitude.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.SelectCommand.Subscribe(x => model.Select(x.Model));

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(model.CompositeDisposable);
		}
	}
}
