﻿using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.ViewModels.Map {
	/// <summary>
	/// マップViewModel
	/// </summary>
	internal class MapViewModel : ViewModelBase {
		/// <summary>
		/// マップコントロール
		/// </summary>
		public IReadOnlyReactiveProperty<IMapControl> MapControl {
			get;
		}

		/// <summary>
		/// マップ用アイテムグループリスト
		/// </summary>
		public ReadOnlyReactiveCollection<MapPinViewModel> ItemsForMapView {
			get;
		}

		/// <summary>
		/// ポインター
		/// </summary>
		public IReadOnlyReactiveProperty<MapPinViewModel> Pointer {
			get;
		}

		/// <summary>
		/// ポインター座標
		/// </summary>
		public IReadOnlyReactiveProperty<GpsLocation> PointerLocation {
			get;
		}

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public IReadOnlyReactiveProperty<string> BingMapApiKey {
			get;
		}

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public IReadOnlyReactiveProperty<int> MapPinSize {
			get;
		}

		/// <summary>
		/// 拡大
		/// </summary>
		public IReactiveProperty<double> ZoomLevel {
			get;
		}

		/// <summary>
		/// 中心座標
		/// </summary>
		public IReactiveProperty<GpsLocation> CenterLocation {
			get;
		}

		/// <summary>
		/// マップ上のピン選択コマンド
		/// </summary>
		public ReactiveCommand<MapPinViewModel> SelectCommand {
			get;
		} = new ReactiveCommand<MapPinViewModel>();

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
			this.PointerLocation = model.PointerLocation.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.BingMapApiKey = model.BingMapApiKey.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.MapPinSize = model.MapPinSize.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.ZoomLevel = model.ZoomLevel.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.CenterLocation = model.CenterLocation.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.SelectCommand.Subscribe(x => model.Select(x.Model));

			// モデル破棄時にこのインスタンスも破棄
			this.AddTo(model.CompositeDisposable);
		}
	}
}
