using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Input;

using Livet;

using Microsoft.Maps.MapControl.WPF;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Map {
	internal class MapModel : MediaFileCollection {
		/// <summary>
		/// 中心座標変更通知用Subject
		/// </summary>
		private readonly Subject<GpsLocation> _onCenterLocationChanged = new Subject<GpsLocation>();

		/// <summary>
		/// 中心座標変更通知
		/// </summary>
		public IObservable<GpsLocation> OnCenterLocationChanged {
			get {
				return this._onCenterLocationChanged.AsObservable();
			}
		}

		/// <summary>
		/// マップコントロール(GUIパーツ)
		/// </summary>
		public IReactiveProperty<IMapControl> MapControl {
			get;
		} = new ReactivePropertySlim<IMapControl>();

		/// <summary>
		/// カレント(複数)
		/// </summary>
		/// <remarks>
		/// 選択中のファイル
		/// </remarks>
		public IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		}

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
		public IReactiveProperty<IEnumerable<MapPin>> ItemsForMapView {
			get;
		} = new ReactivePropertySlim<IEnumerable<MapPin>>(Array.Empty<MapPin>());

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

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="items">他所で生成したメディアファイルリスト</param>
		/// <param name="selectedItems">他所で生成した選択中メディアファイルリスト</param>
		public MapModel(ObservableSynchronizedCollection<IMediaFileModel> items, IReactiveProperty<IEnumerable<IMediaFileModel>> selectedItems) : base(items) {
			// マップコントロール(GUIパーツ)
			this.MapControl.Value = Get.Instance<IMapControl>();

			// Bing Map Api Key
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// マップピンサイズ
			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.CurrentMediaFiles = selectedItems;

			this.Items
				.CollectionChangedAsObservable()
				.Throttle(TimeSpan.FromMilliseconds(100))
				.Subscribe(_ => {
					using (this.DisposeLock.DisposableEnterReadLock()) {
						if (this.DisposeState != DisposeState.NotDisposed) {
							return;
						}
						// マップ表示の場合はコード側でズームレベルの変更をしない。
						if (this.Settings.GeneralSettings.DisplayMode.Value == DisplayMode.Map) {
							return;
						}
						var maxLat = -90d;
						var minLat = 90d;
						var maxLon = -180d;
						var minLon = 180d;
						foreach (var item in this.Items.Where(x => x.Location != null)) {
							maxLat = Math.Max(item.Location.Latitude, maxLat);
							minLat = Math.Min(item.Location.Latitude, minLat);
							maxLon = Math.Max(item.Location.Longitude, maxLon);
							minLon = Math.Min(item.Location.Longitude, minLon);
						}
						this.MapControl.Value.SetViewArea(
							new Location(minLat, maxLon),
							new Location(maxLat, minLon),
							(int)(this.Settings.GeneralSettings.MapPinSize.Value * 1.2)
						);
					}
				});


			// ファイル、無視ファイル、アイテム内座標などが変わったときにマップ用アイテムグループリストを更新
			Observable.FromEventPattern<MapEventArgs>(
					h => this.MapControl.Value.ViewChangeOnFrame += h,
					h => this.MapControl.Value.ViewChangeOnFrame -= h
				).ToUnit()
				.Merge(this.Items.ToCollectionChanged<IMediaFileModel>().ToUnit())
				//.Merge(this.Items.ObserveElementProperty(x => x.Location, false).ToUnit())
				.Sample(TimeSpan.FromSeconds(1))
				.Merge(this.IgnoreMediaFiles.ToUnit())
				.Merge(this.MapControl.Value.Ready)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(_ => {
					using (this.DisposeLock.DisposableEnterReadLock()) {
						if (this.DisposeState != DisposeState.NotDisposed) {
							return;
						}
						this.UpdateItemsForMapView();
					}
				}).AddTo(this.CompositeDisposable);

			// カレントアイテム変化時、ピンステータスの書き換え
			this.CurrentMediaFiles.Subscribe(_ => {
				foreach (var mg in this.ItemsForMapView.Value) {
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
			var hasError = map.HasAreaPropertyError;

			// マップコントロールの表示範囲座標の取得
			var west = map.West;
			var east = map.East;
			var north = map.North;
			var south = map.South;

			foreach (var item in this.Items) {
				if (this.IgnoreMediaFiles.Value.Contains(item)) {
					continue;
				}
				if (!(item.Location is { } location)) {
					continue;
				}

				if (
					!hasError &&
					(
						north < location.Latitude ||
						south > location.Latitude ||
						west > location.Longitude ||
						east < location.Longitude
					)) {
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

			this.ItemsForMapView.Value = list;
		}

		/// <summary>
		/// 選択コマンド
		/// </summary>
		/// <param name="mediaGroup"></param>
		public void Select(MapPin mediaGroup) {
			this.CurrentMediaFiles.Value = mediaGroup.Items.ToArray();
		}

		public override string ToString() {
			return $"<[{base.ToString()}] ({this.Count.Value})>";
		}
	}
}
