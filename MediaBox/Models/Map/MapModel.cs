using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;

using Microsoft.Maps.MapControl.WPF;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class MapModel : MediaFileCollection {

		private readonly Subject<IEnumerable<MediaFile>> _onSelect = new Subject<IEnumerable<MediaFile>>();
		private readonly FilterDescriptionManager _filterDescriptionManager;
		/// <summary>
		/// GPS登録完了通知
		/// </summary>
		public IObservable<IEnumerable<MediaFile>> OnSelect {
			get {
				return this._onSelect.AsObservable();
			}
		}

		/// <summary>
		/// マップコントロール(GUIパーツ)
		/// </summary>
		public ReactivePropertySlim<IMapControl> MapControl {
			get;
		} = new ReactivePropertySlim<IMapControl>();

		/// <summary>
		/// カレント
		/// </summary>
		public ReactivePropertySlim<MediaFile> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<MediaFile>();

		/// <summary>
		/// カレント(複数)
		/// </summary>
		public ReactivePropertySlim<IEnumerable<MediaFile>> CurrentMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFile>>(Array.Empty<MediaFile>());

		/// <summary>
		/// 無視ファイル
		/// </summary>
		public ReactivePropertySlim<IEnumerable<MediaFile>> IgnoreMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<MediaFile>>(Array.Empty<MediaFile>());

		/// <summary>
		/// マップ用アイテムグループリスト
		/// </summary>
		public ReactiveCollection<MapPin> ItemsForMapView {
			get;
		} = new ReactiveCollection<MapPin>(UIDispatcherScheduler.Default);

		/// <summary>
		/// マウスポインター追跡用メディアグループ
		/// </summary>
		public ReactivePropertySlim<MapPin> Pointer {
			get;
		} = new ReactivePropertySlim<MapPin>();

		/// <summary>
		/// マウスポインターGPS座標 緯度
		/// </summary>
		public ReactivePropertySlim<double> PointerLatitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// マウスポインターGPS座標 経度
		/// </summary>
		public ReactivePropertySlim<double> PointerLongitude {
			get;
		} = new ReactivePropertySlim<double>();

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
		/// 拡大レベル
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

		public MapModel() {
			this._filterDescriptionManager = Get.Instance<FilterDescriptionManager>();
			// マップコントロール(GUIパーツ)
			this.MapControl.Value = Get.Instance<IMapControl>();

			// Bing Map Api Key
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// マップピンサイズ
			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// 拡大レベル
			this.ZoomLevel =
				this.CurrentMediaFile.ToUnit()
					.Merge(this.Settings.GeneralSettings.DisplayMode.ToUnit())
					.Where(_ => this.Settings.GeneralSettings.DisplayMode.Value != DisplayMode.Map)
					.Select(x => this.CurrentMediaFile.Value?.Latitude != null && this.CurrentMediaFile.Value.Longitude != null ? 14d : 0d)
					.ToReactiveProperty()
					.AddTo(this.CompositeDisposable);

			// 中心座標
			// カレントアイテムがあればそのアイテムの座標、なければ全アイテムのうち、緯度経度の揃っているものを一つピックアップしてその座標
			// 中心座標 緯度
			this.CenterLatitude =
				this.CurrentMediaFile.ToUnit()
					.Merge(this.Items.CollectionChangedAsObservable().ToUnit())
					.Merge(this.Settings.GeneralSettings.DisplayMode.ToUnit())
					.Where(_ => this.Settings.GeneralSettings.DisplayMode.Value != DisplayMode.Map)
					.Select(_ =>
						new[] { this.CurrentMediaFile.Value }
							.Union(this.Items.Take(1).ToArray())
							.FirstOrDefault(x => x?.Latitude != null && x.Longitude != null)
							?.Latitude ?? 0)
					.ToReactiveProperty()
					.AddTo(this.CompositeDisposable);

			// 中心座標 経度
			this.CenterLongitude =
				this.CurrentMediaFile.ToUnit()
					.Merge(this.Items.CollectionChangedAsObservable().ToUnit())
					.Merge(this.Settings.GeneralSettings.DisplayMode.ToUnit())
					.Where(_ => this.Settings.GeneralSettings.DisplayMode.Value != DisplayMode.Map)
					.Select(_ =>
						new[] { this.CurrentMediaFile.Value }
							.Union(this.Items.Take(1).ToArray())
							.FirstOrDefault(x => x?.Latitude != null && x.Longitude != null)
							?.Longitude ?? 0)
					.ToReactiveProperty()
					.AddTo(this.CompositeDisposable);

			// ファイル、無視ファイル、アイテム内座標などが変わったときにマップ用アイテムグループリストを更新
			Observable.FromEventPattern<MapEventArgs>(
					h => this.MapControl.Value.ViewChangeOnFrame += h,
					h => this.MapControl.Value.ViewChangeOnFrame -= h
				).ToUnit()
				.Merge(this.Items.ToCollectionChanged().ToUnit())
				.Merge(this._filterDescriptionManager.OnUpdateFilteringConditions)
				.Merge(this.Items.ObserveElementProperty(x => x.Latitude, false).ToUnit())
				.Merge(this.Items.ObserveElementProperty(x => x.Longitude, false).ToUnit())
				.Merge(Observable.Return(Unit.Default))
				.Sample(TimeSpan.FromSeconds(1))
				.Merge(this.IgnoreMediaFiles.ToUnit())
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Subscribe(_ => {
					this.UpdateItemsForMapView();
				}).AddTo(this.CompositeDisposable);

			// カレントアイテム変化時、ピンステータスの書き換え
			this.CurrentMediaFiles.Subscribe(_ => {
				foreach (var mg in this.ItemsForMapView.ToArray()) {
					if (mg.Items.All(x => this.CurrentMediaFiles.Value.Contains(x))) {
						mg.PinState.Value = PinState.Selected;
					} else if (mg.Items.Any(x => this.CurrentMediaFiles.Value.Contains(x))) {
						mg.PinState.Value = PinState.Indeterminate;
					} else {
						mg.PinState.Value = PinState.Unselected;
					}
				}
			});
		}

		/// <summary>
		/// マップ用アイテムグループリスト更新
		/// </summary>
		private void UpdateItemsForMapView() {
			var list = new List<MapPin>();

			var map = this.MapControl.Value;
			var leftTop = map.ViewportPointToLocation(new Point(-this.MapPinSize.Value / 2d, -this.MapPinSize.Value / 2d));
			var rightBottom = map.ViewportPointToLocation(new Point(map.ActualWidth + (this.MapPinSize.Value / 2d), map.ActualHeight + (this.MapPinSize.Value / 2d)));
			foreach (var item in this.Items.ToArray()) {
				if (this.IgnoreMediaFiles.Value.Contains(item)) {
					continue;
				}
				if (!(item.Latitude is double latitude) || !(item.Longitude is double longitude)) {
					continue;
				}
				// フィルタリング条件
				if (!this._filterDescriptionManager.Filter(item)) {
					continue;
				}
				if (
					leftTop.Latitude < latitude ||
					rightBottom.Latitude > latitude ||
					leftTop.Longitude > longitude ||
					rightBottom.Longitude < longitude) {
					continue;
				}
				var topLeft = new Location(latitude, longitude);
				var rect =
					new Rectangle(
						map.LocationToViewportPoint(topLeft),
						new Size(this.MapPinSize.Value, this.MapPinSize.Value)
					);
				var cores = list.Where(x => rect.IntersectsWith(x.CoreRectangle)).ToList();
				if (cores.Count == 0) {
					list.Add(Get.Instance<MapPin>(item, rect));
				} else {
					cores.OrderBy(x => rect.DistanceTo(x.CoreRectangle)).First().Items.Add(item);
				}
			}

			foreach (var mg in list) {
				if (mg.Items.All(this.CurrentMediaFiles.Value.Contains)) {
					mg.PinState.Value = PinState.Selected;
				} else if (mg.Items.Any(this.CurrentMediaFiles.Value.Contains)) {
					mg.PinState.Value = PinState.Indeterminate;
				} else {
					mg.PinState.Value = PinState.Unselected;
				}
			}

			this.ItemsForMapView.ClearOnScheduler();
			this.ItemsForMapView.AddRangeOnScheduler(list);
		}

		/// <summary>
		/// 選択コマンド
		/// </summary>
		/// <param name="mediaGroup"></param>
		public void Select(MapPin mediaGroup) {
			this._onSelect.OnNext(mediaGroup.Items);
		}
	}
}
