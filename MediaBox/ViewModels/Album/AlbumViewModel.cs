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
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;


namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムViewModel
	/// </summary>
	internal class AlbumViewModel : MediaFileCollectionViewModel<Models.Album.Album> {

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public ReactiveProperty<string> Title {
			get;
		}

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
		public ReactiveCollection<MediaFileViewModel> SelectedMediaFiles {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		/// <summary>
		/// GPS設定用VM
		/// </summary>
		public ReactivePropertySlim<GpsSelectorViewModel> GpsSelectorViewModel {
			get;
		} = new ReactivePropertySlim<GpsSelectorViewModel>(Get.Instance<GpsSelectorViewModel>());

		public ReactivePropertySlim<bool> SelectGpsMode {
			get;
		} = new ReactivePropertySlim<bool>();

		/// <summary>
		/// GPS設定コマンド
		/// </summary>
		public ReactiveCommand SetGpsCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 複数メディアファイルまとめてプロパティ表示用ViewModel
		/// </summary>
		public ReactiveProperty<MediaFilePropertiesViewModel> MediaFilePropertiesViewModel {
			get;
		} = new ReactiveProperty<MediaFilePropertiesViewModel>(Get.Instance<MediaFilePropertiesViewModel>());

		/// <summary>
		/// 選択中メディアファイル
		/// </summary>
		public ReactivePropertySlim<MediaFileViewModel> CurrentItem {
			get;
		} = new ReactivePropertySlim<MediaFileViewModel>(mode: ReactivePropertyMode.RaiseLatestValueOnSubscribe);

		/// <summary>
		/// 表示モード
		/// </summary>
		public ReactivePropertySlim<DisplayMode> DisplayMode {
			get;
		} = new ReactivePropertySlim<DisplayMode>();

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
		public AlbumViewModel(Models.Album.Album model) : base(model) {
			this.Title = this.Model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.MonitoringDirectories = this.Model.MonitoringDirectories.ToReadOnlyReactiveCollection();

			this.SelectedMediaFiles
				.ToCollectionChanged()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							this.MediaFilePropertiesViewModel.Value.Add(x.Value);
							this.GpsSelectorViewModel.Value.SelectedMediaFiles.Add(x.Value);
							break;
						case NotifyCollectionChangedAction.Remove:
							this.MediaFilePropertiesViewModel.Value.Remove(x.Value);
							this.GpsSelectorViewModel.Value.SelectedMediaFiles.Remove(x.Value);
							break;
					}
				});

			this.SetGpsCommand.Subscribe(() => {
				this.SelectGpsMode.Value = !this.SelectGpsMode.Value;
			});

			this.CurrentItem
				.ToOldAndNewValue()
				.CombineLatest(
					this.DisplayMode.Where(x =>x == Album.DisplayMode.Detail),
					(currentItem, displayMode)=>(currentItem, displayMode))
				.Subscribe(x => {
					x.currentItem.OldValue?.UnloadImageCommand.Execute();
					x.currentItem.NewValue.LoadImageCommand.Execute();
				});

			// 表示モード変更コマンド
			this.ChangeDisplayModeCommand.Subscribe(x => {
				this.DisplayMode.Value = x;
			});

			// 拡大
			this.ZoomLevel = this.CurrentItem.Where(x => x != null).Select(x => x.Latitude.Value != null && x.Longitude.Value != null ? 14d : 0d).ToReactiveProperty();

			// 中心座標 緯度
			this.CenterLatitude =
				this.CurrentItem
					.CombineLatest(this.Items.CollectionChangedAsObservable(),
					(item, _) => item)
					.Select(item => item?.Latitude.Value ?? this.Items.FirstOrDefault(x => x.Latitude.Value != null && x.Longitude.Value != null)?.Latitude.Value ?? 0)
					.ToReactiveProperty();

			// 中心座標 経度
			this.CenterLongitude =
				this.CurrentItem
					.CombineLatest(this.Items.CollectionChangedAsObservable(),
					(item, _) => item)
					.Select(item => item?.Longitude.Value ?? this.Items.FirstOrDefault(x => x.Latitude.Value != null && x.Longitude.Value != null)?.Longitude.Value ?? 0)
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
					.Merge(this.Items.ToCollectionChanged().ToUnit())
					.Merge(Observable.Return(Unit.Default))
					.ObserveOn(TaskPoolScheduler.Default)
					.Subscribe(_ => {
						var list = new List<MediaGroupViewModel>();
						// TODO : マップ範囲内のメディアのみを対象にする
						foreach (var item in this.Items) {
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
								cores.OrderBy(x => rect.DistanceTo(x.CoreRectangle)).First().Add(item);
							}
						}
						this.ItemsForMapView.ClearOnScheduler();
						this.ItemsForMapView.AddRangeOnScheduler(list);
					});
				}).AddTo(this.CompositeDisposable);

			this.GpsSelectorViewModel.Value.Items.AddRange(this.Items);

			this.Items
				.ToCollectionChanged()
				.ObserveOnUIDispatcher()
				.Subscribe(x => {
					// GPS選択同期
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							this.GpsSelectorViewModel.Value.Items.Add(x.Value);
							break;
						case NotifyCollectionChangedAction.Remove:
							this.GpsSelectorViewModel.Value.Items.Remove(x.Value);
							break;
					}
				});

			this.CurrentItem.Where(x => x != null).Subscribe(x => {
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
