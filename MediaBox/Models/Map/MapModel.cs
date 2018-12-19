﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class MapModel : MediaFileCollection {
		/// <summary>
		/// マップコントロール(GUIパーツ)
		/// </summary>
		public ReactivePropertySlim<MapControl> MapControl {
			get;
		} = new ReactivePropertySlim<MapControl>();

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
			this.MapControl.Value = new MapControl();
			
			// Bing Map Api Key
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReadOnlyReactivePropertySlim();

			// マップピンサイズ
			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReadOnlyReactivePropertySlim();

			// 拡大レベル
			this.ZoomLevel = this.CurrentMediaFile.Where(x => x != null).Select(x => x.Latitude.Value != null && x.Longitude.Value != null ? 14d : 0d).ToReactiveProperty();


			// 中心座標
			// カレントアイテムがあればそのアイテムの座標、なければ全アイテムのうち、緯度経度の揃っているものを一つピックアップしてその座標
			// 中心座標 緯度
			this.CenterLatitude =
				this.CurrentMediaFile
					.CombineLatest(this.Items.CollectionChangedAsObservable(),
					(item, _) => item)
					.Select(item => item?.Latitude.Value ?? this.Items.FirstOrDefault(x => x.Latitude.Value != null && x.Longitude.Value != null)?.Latitude.Value ?? 0)
					.ToReactiveProperty();

			// 中心座標 経度
			this.CenterLongitude =
				this.CurrentMediaFile
					.CombineLatest(this.Items.CollectionChangedAsObservable(),
					(item, _) => item)
					.Select(item => item?.Longitude.Value ?? this.Items.FirstOrDefault(x => x.Latitude.Value != null && x.Longitude.Value != null)?.Longitude.Value ?? 0)
					.ToReactiveProperty();

			// ファイル、無視ファイル、アイテム内座標などが変わったときにマップ用アイテムグループリストを更新
			Observable.FromEventPattern<MapEventArgs>(
					h => this.MapControl.Value.ViewChangeOnFrame += h,
					h => this.MapControl.Value.ViewChangeOnFrame -= h
				).ToUnit()
				.Merge(this.Items.ToCollectionChanged().ToUnit())
				.Merge(this.IgnoreMediaFiles.ToCollectionChanged().ToUnit())
				.Merge(Observable.Return(Unit.Default))
				.Sample(TimeSpan.FromSeconds(1))
				.ObserveOnUIDispatcher()
				.Subscribe(_ => {
					this.UpdateItemsForMapView();
				}).AddTo(this.CompositeDisposable);
			
			this.Items
				.ToCollectionChanged()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							x.Value
								.Latitude.Skip(1)
								.Merge(x.Value.Longitude.Skip(1))
								.Subscribe(_ => this.UpdateItemsForMapView());
							break;
						case NotifyCollectionChangedAction.Remove:
							// TODO : イベント外す処理を追加する
							break;
					}
				});
		}

		/// <summary>
		/// マップ用アイテムグループリスト更新
		/// </summary>
		private void UpdateItemsForMapView() {
			var list = new List<MediaGroup>();
			// TODO : マップ範囲内のメディアのみを対象にする
			foreach (var item in this.Items) {
				if (this.IgnoreMediaFiles.Contains(item)) {
					continue;
				}
				if (!(item.Latitude.Value is double latitude) || !(item.Longitude.Value is double longitude)) {
					continue;
				}
				var topLeft = new Location(latitude, longitude);
				var rect =
					new Rectangle(
						this.MapControl.Value.LocationToViewportPoint(topLeft),
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
