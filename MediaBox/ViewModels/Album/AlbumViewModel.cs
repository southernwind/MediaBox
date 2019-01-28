using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムViewModel
	/// </summary>
	internal class AlbumViewModel : MediaFileCollectionViewModel<AlbumModel> {
		private AlbumSelectorViewModel _albumSelector;

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// 選択中メディアファイル
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// カレントメディアファイル
		/// </summary>
		public IReadOnlyReactiveProperty<IMediaFileViewModel> CurrentItem {
			get;
		}

		/// <summary>
		/// 複数メディアファイルまとめて情報表示用ViewModel
		/// </summary>
		public IReadOnlyReactiveProperty<MediaFileInformationsViewModel> MediaFileInformations {
			get;
		}

		/// <summary>
		/// 表示モード
		/// </summary>
		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

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
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}

		/// <summary>
		/// アルバムセレクター
		/// </summary>
		public AlbumSelectorViewModel AlbumSelector {
			get {
				// TODO : AlbumSelectorViewModelのコンストラクタでAlbumViewModelを生成していて、こっちのコンストラクタでもAlbumSelectorViewModelを生成してしまうと
				// TODO : StackOverFlowが起こるため、生成を遅らせる。生成が遅れてしまえばViewModelFactoryからAlbumViewModelを取得できるので、コンストラクタを通らずに済むので応急措置。
				return this._albumSelector ?? (this._albumSelector = Get.Instance<AlbumSelectorViewModel>());
			}
		}

		/// <summary>
		/// ファイル追加コマンド
		/// </summary>
		public ReactiveCommand<IEnumerable<IMediaFileViewModel>> AddMediaFileCommand {
			get;
		} = new ReactiveCommand<IEnumerable<IMediaFileViewModel>>();

		/// <summary>
		/// ソート順制御
		/// </summary>
		public SortDescriptionManagerViewModel SortDescriptionManager {
			get;
		} = Get.Instance<SortDescriptionManagerViewModel>();

		/// <summary>
		/// フィルター設定ウィンドウオープン
		/// </summary>
		public ReactiveCommand OpenSetFilterWindowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public AlbumViewModel(AlbumModel model) : base(model) {
			this.Title = this.Model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.MonitoringDirectories = this.Model.MonitoringDirectories.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.Map = this.Model.Map.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.MediaFileInformations =
				this.Model
					.MediaFileInformations
					.Select(this.ViewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			this.DisplayMode = this.Model.DisplayMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.CurrentItem =
				this.Model
					.CurrentMediaFile
					.Select(x => x == null ? null : this.ViewModelFactory.Create(x))
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			// VM⇔Model間双方向同期
			this.SelectedMediaFiles.TwoWaySynchronize(
				this.Model.CurrentMediaFiles,
				x => x.Select(vm => vm.Model).ToArray(),
				x => x.Select(this.ViewModelFactory.Create).ToArray());

			// 表示モード変更コマンド
			this.ChangeDisplayModeCommand
				.Subscribe(this.Model.ChangeDisplayMode)
				.AddTo(this.CompositeDisposable);

			// ファイル追加コマンド
			this.AddMediaFileCommand.Subscribe(x => {
				if (this.Model is RegisteredAlbum ra) {
					ra.AddFiles(x.Select(vm => vm.Model));
				}
			});

			// フィルター設定ウィンドウオープンコマンド
			this.OpenSetFilterWindowCommand.Subscribe(() => {
				var vm = Get.Instance<FilterDescriptionManagerViewModel>();
				var message = new TransitionMessage(typeof(Views.Album.Filter.SetFilterWindow), vm, TransitionMode.Normal);
				this.Messenger.Raise(message);
			});

			var itemsCollectionView = CollectionViewSource.GetDefaultView(this.Items);
			// ソート再設定
			this.Settings.GeneralSettings.SortDescriptions.Subscribe(x => {
				itemsCollectionView.SortDescriptions.Clear();
				foreach (var si in x) {
					itemsCollectionView.SortDescriptions.Add(new SortDescription(si.PropertyName, si.Direction));
				}
			});
			var fdm = Get.Instance<FilterDescriptionManager>();
			// フィルター再設定
			fdm.OnUpdateFilteringConditions.Subscribe(_ => {
				itemsCollectionView.Filter = x => {
					if (x is IMediaFileViewModel vm) {
						return fdm.Filter(vm.Model);
					}
					return false;
				};
			});

		}
	}
}
