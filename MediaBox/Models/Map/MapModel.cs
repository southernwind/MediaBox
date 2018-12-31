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

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class MapModel : MediaFileCollection {
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
		/// 無視ファイル
		/// </summary>
		public ReactiveCollection<MediaFile> IgnoreMediaFiles {
			get;
		} = new ReactiveCollection<MediaFile>();

		/// <summary>
		/// マップ用アイテムグループリスト
		/// </summary>
		public ReactiveCollection<MediaGroup> ItemsForMapView {
			get;
		} = new ReactiveCollection<MediaGroup>(UIDispatcherScheduler.Default);

		/// <summary>
		/// マウスポインター追跡用メディアグループ
		/// </summary>
		public ReactivePropertySlim<MediaGroup> Pointer {
			get;
		} = new ReactivePropertySlim<MediaGroup>();

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
			// マップコントロール(GUIパーツ)
			this.MapControl.Value = Get.Instance<IMapControl>();

			// Bing Map Api Key
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReadOnlyReactivePropertySlim();

			// マップピンサイズ
			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReadOnlyReactivePropertySlim();

			// 拡大レベル
			this.ZoomLevel = this.CurrentMediaFile.Select(x => x?.Latitude.Value != null && x.Longitude.Value != null ? 14d : 0d).ToReactiveProperty();


			// 中心座標
			// カレントアイテムがあればそのアイテムの座標、なければ全アイテムのうち、緯度経度の揃っているものを一つピックアップしてその座標
			// 中心座標 緯度
			this.CenterLatitude =
				this.CurrentMediaFile.ToUnit()
					.Merge(this.Items.CollectionChangedAsObservable().ToUnit())
					.Select(_ =>
						new[] { this.CurrentMediaFile.Value }
							.Union(this.Items.Take(1).ToArray())
							.FirstOrDefault(x => x?.Latitude.Value != null && x.Longitude.Value != null)
							?.Latitude.Value ?? 0)
					.ToReactiveProperty();

			// 中心座標 経度
			this.CenterLongitude =
				this.CurrentMediaFile.ToUnit()
					.Merge(this.Items.CollectionChangedAsObservable().ToUnit())
					.Select(_ =>
						new[] { this.CurrentMediaFile.Value }
							.Union(this.Items.Take(1).ToArray())
							.FirstOrDefault(x => x?.Latitude.Value != null && x.Longitude.Value != null)
							?.Longitude.Value ?? 0)
					.ToReactiveProperty();

			var update = new Subject<Unit>();
			update
				.Sample(TimeSpan.FromSeconds(1))
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Subscribe(x => {
					this.UpdateItemsForMapView();
				});
			// ファイル、無視ファイル、アイテム内座標などが変わったときにマップ用アイテムグループリストを更新
			Observable.FromEventPattern<MapEventArgs>(
					h => this.MapControl.Value.ViewChangeOnFrame += h,
					h => this.MapControl.Value.ViewChangeOnFrame -= h
				).ToUnit()
				.Merge(this.Items.ToCollectionChanged().ToUnit())
				.Merge(this.IgnoreMediaFiles.ToCollectionChanged().ToUnit())
				.Merge(Observable.Return(Unit.Default))
				.Subscribe(_ => {
					update.OnNext(Unit.Default);
				}).AddTo(this.CompositeDisposable);

			this
				.Items
				.ToReadOnlyReactiveCollection(x =>
					x.Latitude
						.Skip(1)
						.Merge(x.Longitude.Skip(1))
						.Subscribe(_ => update.OnNext(Unit.Default))
						.AddTo(this.CompositeDisposable)
				)
				.AddTo(this.CompositeDisposable)
				.DisposeWhenRemove()
				.AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// マップ用アイテムグループリスト更新
		/// </summary>
		private void UpdateItemsForMapView() {
			var list = new List<MediaGroup>();

			var map = this.MapControl.Value;
			var leftTop = map.ViewportPointToLocation(new Point(-this.MapPinSize.Value / 2, -this.MapPinSize.Value / 2));
			var rightBottom = map.ViewportPointToLocation(new Point(map.ActualWidth + (this.MapPinSize.Value / 2), map.ActualHeight + (this.MapPinSize.Value / 2)));
			foreach (var item in this.Items.ToArray()) {
				if (this.IgnoreMediaFiles.Contains(item)) {
					continue;
				}
				if (!(item.Latitude.Value is double latitude) || !(item.Longitude.Value is double longitude)) {
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
					list.Add(Get.Instance<MediaGroup>(item, rect));
				} else {
					cores.OrderBy(x => rect.DistanceTo(x.CoreRectangle)).First().Items.Add(item);
				}
			}
			this.ItemsForMapView.ClearOnScheduler();
			this.ItemsForMapView.AddRangeOnScheduler(list);
		}
	}
}
