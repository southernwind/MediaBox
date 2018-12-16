using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Microsoft.Maps.MapControl.WPF;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels.Media {
	internal class GpsSelectorViewModel : ViewModelBase {
		/// <summary>
		/// 緯度
		/// </summary>
		public ReactivePropertySlim<double> Latitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// 経度
		/// </summary>
		public ReactivePropertySlim<double> Longitude {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public ReadOnlyReactivePropertySlim<string> BingMapApiKey {
			get;
		}

		public ReactiveCollection<MediaFileViewModel> Items {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		public ReactiveCollection<MediaFileViewModel> SelectedMediaFiles {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();

		public ReactivePropertySlim<MediaGroupViewModel> TargetFiles {
			get;
		} = new ReactivePropertySlim<MediaGroupViewModel>();

		/// <summary>
		/// マップ表示用グルーピング済みメディアファイルViewModelリスト
		/// </summary>
		public ReactiveCollection<MediaGroupViewModel> ItemsForMapView {
			get;
		} = new ReactiveCollection<MediaGroupViewModel>();

		/// <summary>
		/// マップコントロール
		/// </summary>
		public ReactiveProperty<MapCore> Map {
			get;
		} = new ReactiveProperty<MapCore>();

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public ReadOnlyReactivePropertySlim<int> MapPinSize {
			get;
		}

		public ReactiveCommand SetGpsCommand {
			get;
		} = new ReactiveCommand();

		public GpsSelectorViewModel() {
			this.BingMapApiKey = this.Settings.GeneralSettings.BingMapApiKey.ToReadOnlyReactivePropertySlim();
			this.MapPinSize = this.Settings.GeneralSettings.MapPinSize.ToReadOnlyReactivePropertySlim();
			
			this.SetGpsCommand.Subscribe(() => {
				foreach (var item in this.TargetFiles.Value.Items) {
					item.SetGpsCommand.Execute((this.Latitude.Value, this.Longitude.Value));
				}
				this.TargetFiles.Value = null;
			});

			this.SelectedMediaFiles
				.ToCollectionChanged()
				.Subscribe(_ => {
					if (this.SelectedMediaFiles.Count == 0) {
						return;
					}
					this.TargetFiles.Value = Get.Instance<MediaGroupViewModel>(
						this.SelectedMediaFiles.First(), default(Rectangle));
					foreach (var item in this.SelectedMediaFiles.Skip(1)) {
						this.TargetFiles.Value.Add(item);
					}
				});

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
						.Merge(this.SelectedMediaFiles.ToCollectionChanged().ToUnit())
						.Merge(Observable.Return(Unit.Default))
						.ObserveOnUIDispatcher()
						.Subscribe(_ => {
							this.UpdateItemsForMapView();
						});
				}).AddTo(this.CompositeDisposable);

			this.Items
				.ToCollectionChanged()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							x.Value
								.Latitude
								.Merge(x.Value.Longitude)
								.Subscribe(_ => this.UpdateItemsForMapView());
							break;
						case NotifyCollectionChangedAction.Remove:
							// TODO : イベント外す処理を追加する
							break;
					}
				});
		}

		private void UpdateItemsForMapView() {
			if (this.Map.Value == null) {
				return;
			}
			var list = new List<MediaGroupViewModel>();
			// TODO : マップ範囲内のメディアのみを対象にする
			foreach (var item in this.Items) {
				if (this.SelectedMediaFiles.Contains(item)) {
					continue;
				}
				if (!(item.Latitude.Value is double latitude) || !(item.Longitude.Value is double longitude)) {
					continue;
				}
				var topLeft = new Location(latitude, longitude);
				var rect =
					new Rectangle(
						this.Map.Value.LocationToViewportPoint(topLeft),
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
		}
	}
}
