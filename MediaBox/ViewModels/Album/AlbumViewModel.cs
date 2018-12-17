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
using SandBeige.MediaBox.ViewModels.Map;
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
		/// 選択中メディアファイル
		/// </summary>
		public ReactiveCollection<MediaFileViewModel> SelectedMediaFiles {
			get;
		} = new ReactiveCollection<MediaFileViewModel>();
		
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

		public ReadOnlyReactivePropertySlim<MapViewModel> Map {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AlbumViewModel(Models.Album.Album model) : base(model) {
			this.Title = this.Model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.MonitoringDirectories = this.Model.MonitoringDirectories.ToReadOnlyReactiveCollection();

			this.Map = this.Model.Map.Select(x => Get.Instance<MapViewModel>(x)).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.SelectedMediaFiles
				.ToCollectionChanged()
				.Subscribe(x => {
					switch (x.Action) {
						case NotifyCollectionChangedAction.Add:
							this.MediaFilePropertiesViewModel.Value.Add(x.Value);
							break;
						case NotifyCollectionChangedAction.Remove:
							this.MediaFilePropertiesViewModel.Value.Remove(x.Value);
							break;
					}
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
			
			this.CurrentItem.Where(x => x != null).Subscribe(x => {
				x.ExifLoadCommand.Execute();
			});
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
