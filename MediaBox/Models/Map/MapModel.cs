using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;

using Microsoft.Maps.MapControl.WPF;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class MapModel : MediaFileCollection {
		/// <summary>
		/// マップ上のピン選択通知用Subject
		/// </summary>
		private readonly Subject<IEnumerable<IMediaFileModel>> _onSelect = new Subject<IEnumerable<IMediaFileModel>>();

		// ピンフィルター
		private readonly FilterDescriptionManager _filterDescriptionManager;

		/// <summary>
		/// マップ上のピン選択通知
		/// </summary>
		public IObservable<IEnumerable<IMediaFileModel>> OnSelect {
			get {
				return this._onSelect.AsObservable();
			}
		}

		/// <summary>
		/// マップコントロール(GUIパーツ)
		/// </summary>
		public IReactiveProperty<IMapControl> MapControl {
			get;
		} = new ReactivePropertySlim<IMapControl>();

		/// <summary>
		/// カレント
		/// </summary>
		/// <remarks>
		/// 代表値となるファイル
		/// マップの初期表示位置や拡大率の決定はこのファイルをもとに行う。
		/// </remarks>
		public IReactiveProperty<IMediaFileModel> CurrentMediaFile {
			get;
		} = new ReactivePropertySlim<IMediaFileModel>();

		/// <summary>
		/// カレント(複数)
		/// </summary>
		/// <remarks>
		/// 選択中のファイル
		/// </remarks>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

		/// <summary>
		/// 無視ファイル
		/// </summary>
		/// <remarks>
		/// マップピンに含めないファイル
		/// </remarks>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> IgnoreMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileModel>>(Array.Empty<IMediaFileModel>());

		/// <summary>
		/// マップ用アイテムグループリスト
		/// </summary>
		public ReactiveCollection<MapPin> ItemsForMapView {
			get;
		} = new ReactiveCollection<MapPin>(UIDispatcherScheduler.Default);

		/// <summary>
		/// マウスポインター追跡用メディアグループ
		/// </summary>
		public IReactiveProperty<MapPin> Pointer {
			get;
		} = new ReactivePropertySlim<MapPin>();

		/// <summary>
		/// マウスポインターGPS座標
		/// </summary>
		public IReactiveProperty<GpsLocation> PointerLocation {
			get;
		} = new ReactivePropertySlim<GpsLocation>();


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
		/// 拡大レベル
		/// </summary>
		public IReactiveProperty<double> ZoomLevel {
			get;
		} = new ReactiveProperty<double>();

		/// <summary>
		/// 中心座標
		/// </summary>
		public IReactiveProperty<GpsLocation> CenterLocation {
			get;
		} = new ReactivePropertySlim<GpsLocation>(new GpsLocation(0, 0));

		/// <summary>
		/// 移動
		/// </summary>
		public IObservable<GpsLocation> OnMove {
			get;
		}

		/// <summary>
		/// 決定
		/// </summary>
		public IObservable<Unit> OnDecide {
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
			// 中心座標
			// カレントアイテムがあればそのアイテムの座標、なければ全アイテムのうち、座標がnullでないものを一つピックアップする
			this.CurrentMediaFile.ToUnit()
				.Merge(this.Items.CollectionChangedAsObservable().ToUnit())
				.Merge(this.Settings.GeneralSettings.DisplayMode.ToUnit())
				.Where(_ => this.Settings.GeneralSettings.DisplayMode.Value != DisplayMode.Map)
				.Subscribe(_ => {
					var location = new[] { this.CurrentMediaFile.Value }
						.Union(this.Items.Take(1).ToArray())
						.FirstOrDefault(x => x?.Location != null)
						?.Location;
					if (location != null) {
						this.CenterLocation.Value = location;
						this.ZoomLevel.Value = 14;
					}
				})
				.AddTo(this.CompositeDisposable);

			// ファイル、無視ファイル、アイテム内座標などが変わったときにマップ用アイテムグループリストを更新
			Observable.FromEventPattern<MapEventArgs>(
					h => this.MapControl.Value.ViewChangeOnFrame += h,
					h => this.MapControl.Value.ViewChangeOnFrame -= h
				).ToUnit()
				.Merge(this.Items.ToCollectionChanged<IMediaFileModel>().ToUnit())
				.Merge(this._filterDescriptionManager.OnUpdateFilteringConditions)
				//.Merge(this.Items.ObserveElementProperty(x => x.Location, false).ToUnit())
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

			// 移動
			this.OnMove = Observable.FromEvent<MouseEventHandler, MouseEventArgs>(
				h => (sender, e) => {
					h(e);
				},
				h => this.MapControl.Value.MouseMove += h,
				h => this.MapControl.Value.MouseMove -= h
				).Select(x => {
					var loc = this.MapControl.Value.ViewportPointToLocation(x.GetPosition(this.MapControl.Value));
					return new GpsLocation(loc.Latitude, loc.Longitude);
				});

			// 決定
			this.OnDecide = Observable.FromEvent<MouseButtonEventHandler, MouseButtonEventArgs>(
				h => (sender, e) => {
					h(e);
					e.Handled = true;
				},
				h => this.MapControl.Value.MouseDoubleClick += h,
				h => this.MapControl.Value.MouseDoubleClick -= h
				).Select(x => Unit.Default);

			// 座標→ポインター座標片方向同期
			this.OnMove.Subscribe(x => this.PointerLocation.Value = x).AddTo(this.CompositeDisposable);

		}

		/// <summary>
		/// マップ用アイテムグループリスト更新
		/// </summary>
		private void UpdateItemsForMapView() {
			var list = new List<MapPin>();

			var map = this.MapControl.Value;

			// マップコントロールの表示範囲座標の取得
			var leftTop = map.ViewportPointToLocation(new Point(-this.MapPinSize.Value / 2d, -this.MapPinSize.Value / 2d));
			var rightBottom = map.ViewportPointToLocation(new Point(map.ActualWidth + (this.MapPinSize.Value / 2d), map.ActualHeight + (this.MapPinSize.Value / 2d)));

			foreach (var item in this.Items.ToArray()) {
				if (this.IgnoreMediaFiles.Value.Contains(item)) {
					continue;
				}
				if (!(item.Location is GpsLocation location)) {
					continue;
				}
				// フィルタリング条件
				if (!this._filterDescriptionManager.Filter(item)) {
					continue;
				}
				if (
					leftTop.Latitude < location.Latitude ||
					rightBottom.Latitude > location.Latitude ||
					leftTop.Longitude > location.Longitude ||
					rightBottom.Longitude < location.Longitude) {
					continue;
				}
				var topLeft = new Location(location.Latitude, location.Longitude);
				// 座標とピンサイズから矩形を生成
				var rect =
					new Rectangle(
						map.LocationToViewportPoint(topLeft),
						new Size(this.MapPinSize.Value, this.MapPinSize.Value)
					);

				// 生成した矩形が既に存在するピンとかぶる位置にあるかを確かめて、被るようであれば
				// 被るピンのうち、最も矩形に近いピンに含める。
				// 被らないなら新しいピンを追加する。
				var cores = list.Where(x => rect.IntersectsWith(x.CoreRectangle)).ToList();
				if (!cores.Any()) {
					list.Add(Get.Instance<MapPin>(item, rect));
				} else {
					cores.OrderBy(x => rect.DistanceTo(x.CoreRectangle)).First().Items.Add(item);
				}
			}

			// ファイルの選択状態をピンの選択状態に反映する
			foreach (var mg in list) {
				if (mg.Items.All(this.CurrentMediaFiles.Value.Contains)) {
					// すべてのファイルを選択中
					mg.PinState.Value = PinState.Selected;
				} else if (mg.Items.Any(this.CurrentMediaFiles.Value.Contains)) {
					// 一部のファイルを選択中
					mg.PinState.Value = PinState.Indeterminate;
				} else {
					// 選択中ファイルなし
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

		public override string ToString() {
			return $"<[{base.ToString()}] {this.CurrentMediaFile.Value} ({this.Count.Value})>";
		}
	}
}
