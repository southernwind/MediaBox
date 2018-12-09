using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.ViewModels.ValidationAttributes;


namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムViewModel
	/// </summary>
	internal class AlbumViewModel : ViewModelBase {

		/// <summary>
		/// アルバムModel
		/// </summary>
		public Models.Album.Album Model {
			get;
			private set;
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public ReactiveProperty<string> Title {
			get;
			private set;
		}

		/// <summary>
		/// 件数
		/// </summary>
		public ReadOnlyReactivePropertySlim<int> Count {
			get;
			private set;
		}


		/// <summary>
		/// メディアファイルViewModelリスト
		/// </summary>
		public ReadOnlyReactiveCollection<MediaFileViewModel> Items {
			get;
			private set;
		}

		/// <summary>
		/// GPS情報を含むメディアファイルViewModelリスト
		/// </summary>
		public ReactiveCollection<MediaFileViewModel> ItemsContainsGps {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		/// <summary>
		/// マップコントロール
		/// </summary>
		public ReactiveProperty<MapCore> Map {
			get;
		} = new ReactiveProperty<MapCore>();

		/// <summary>
		/// マップ表示用グルーピング済みメディアファイルViewModelリスト
		/// </summary>
		public ReactiveCollection<MediaGroupViewModel> ItemsForMapView {
			get;
		} = new ReactiveCollection<MediaGroupViewModel>(UIDispatcherScheduler.Default);

		/// <summary>
		/// 選択中メディアファイル
		/// </summary>
		public ReactivePropertySlim<MediaFileViewModel> CurrentItem {
			get;
		} = new ReactivePropertySlim<MediaFileViewModel>();

		/// <summary>
		/// 表示モード
		/// </summary>
		public ReactivePropertySlim<DisplayMode> DisplayMode {
			get;
		} = new ReactivePropertySlim<DisplayMode>(Album.DisplayMode.Library);

		/// <summary>
		/// 表示モード変更コマンド
		/// </summary>
		public ReactiveCommand<DisplayMode> ChangeDisplayModeCommand {
			get;
		} = new ReactiveCommand<DisplayMode>();

		/// <summary>
		/// ファイル更新監視ディレクトリ
		/// </summary>
		public ReadOnlyReactiveCollection<string> MonitoringDirectories {
			get;
			private set;
		}

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> BingMapApiKey {
			get;
			private set;
		}

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public ReadOnlyReactivePropertySlim<int> MapPinSize {
			get;
			private set;
		}

		/// <summary>
		/// 拡大
		/// </summary>
		public ReactiveProperty<double> ZoomLevel {
			get;
			private set;
		}

		/// <summary>
		/// 中心座標　緯度
		/// </summary>
		public ReactiveProperty<double> CenterLatitude {
			get;
			private set;
		}

		/// <summary>
		/// 中心座標 経度
		/// </summary>
		public ReactiveProperty<double> CenterLongitude {
			get;
			private set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AlbumViewModel(Models.Album.Album model) {
			this.Model = model;

			this.Title = this.Model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.Count = this.Model.Count.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.Items = this.Model.Items.ToReadOnlyReactiveCollection(x => Get.Instance<MediaFileViewModel>(x)).AddTo(this.CompositeDisposable);
			
			this.MonitoringDirectories = this.Model.MonitoringDirectories.ToReadOnlyReactiveCollection();

			// 表示モード変更コマンド
			this.ChangeDisplayModeCommand.Subscribe(x => {
				this.DisplayMode.Value = x;
			});

			// 拡大
			this.ZoomLevel = this.CurrentItem.Where(x => x != null).Select(x => x.Latitude.Value != null && x.Longitude.Value != null ? 14d : 0d).ToReactiveProperty();

			// 中心座標 緯度
			this.CenterLatitude =
				this.CurrentItem
					.CombineLatest(this.ItemsContainsGps.CollectionChangedAsObservable(),
					(item, _) => item)
					.Select(item => item?.Latitude.Value ?? this.ItemsContainsGps.FirstOrDefault()?.Latitude.Value ?? 0)
					.ToReactiveProperty();

			// 中心座標 経度
			this.CenterLongitude =
				this.CurrentItem
					.CombineLatest(this.ItemsContainsGps.CollectionChangedAsObservable(),
					(item, _) => item)
					.Select(item => item?.Longitude.Value ?? this.ItemsContainsGps.FirstOrDefault()?.Longitude.Value ?? 0)
					.ToReactiveProperty();

			// マップの表示切り替えのたびにメディアファイルのグルーピングをやり直す
			this.Map
				.Where(map => map != null)
				.Subscribe(map => {
					// TODO : map切り替えのタイミングでDispose
					Observable.FromEventPattern<MapEventArgs>(
						h => map.ViewChangeOnFrame += h,
						h => map.ViewChangeOnFrame -= h
					).ToUnit()
					.Sample(TimeSpan.FromSeconds(1))
					.Merge(this.ItemsContainsGps.ToCollectionChanged().ToUnit())
					.Merge(Observable.Return(Unit.Default))
					.ObserveOn(TaskPoolScheduler.Default)
					.Subscribe(_ => {
						var list = new List<MediaGroupViewModel>();
						// TODO : マップ範囲内のメディアのみを対象にする
						foreach (var item in this.ItemsContainsGps) {
							var topLeft = new Location(item.Latitude.Value ?? 0, item.Longitude.Value ?? 0);
							var rect =
								new Rectangle(
									map.LocationToViewportPoint(topLeft),
									new Size(this.MapPinSize.Value, this.MapPinSize.Value)
								);
							var cores = list.Where(x => rect.IntersectsWith(x.CoreRectangle)).ToList();
							if (cores.Count == 0) {
								list.Add(Get.Instance<MediaGroupViewModel>(item, rect));
							} else {
								cores.OrderBy(x => rect.DistanceTo(x.CoreRectangle)).First().List.Add(item);
							}
						}
						this.ItemsForMapView.ClearOnScheduler();
						this.ItemsForMapView.AddRangeOnScheduler(list);
					});
				}).AddTo(this.CompositeDisposable);

			this.ItemsContainsGps.AddRange(this.Items.Where(x => x.Latitude.Value.HasValue && x.Longitude.Value.HasValue));
			this.Items
				.ToCollectionChanged()
				.ObserveOnUIDispatcher()
				.Subscribe(x => {
					if (!x.Value.Latitude.Value.HasValue || !x.Value.Longitude.Value.HasValue) {
						x.Value
							.Latitude
							.FirstAsync(l => l.HasValue)
							.Merge(x.Value.Longitude.FirstAsync(l => l.HasValue)
						).FirstAsync(_ =>
							x.Value.Latitude.Value.HasValue &&
							x.Value.Longitude.Value.HasValue
						).Subscribe(_ => {
							this.ItemsContainsGps.AddOnScheduler(x.Value);
						});

						return;
					}
					if (x.Action == NotifyCollectionChangedAction.Add) {
						this.ItemsContainsGps.AddOnScheduler(x.Value);
					}
					if (x.Action == NotifyCollectionChangedAction.Remove) {
						this.ItemsContainsGps.RemoveOnScheduler(x.Value);
					}
				});

			this.CurrentItem.Where(x => x!=null).Subscribe(x => {
				x.ExifLoadCommand.Execute();
			});

			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReadOnlyReactivePropertySlim();

			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReadOnlyReactivePropertySlim();
		}
	}

	/// <summary>
	/// 表示モード
	/// </summary>
	internal enum DisplayMode {
		/// <summary>
		/// ライブラリ表示
		/// </summary>
		Library,
		/// <summary>
		/// 詳細表示
		/// </summary>
		Detail,
		/// <summary>
		/// マップ表示
		/// </summary>
		Map
	}

}
