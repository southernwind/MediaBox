using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Gesture;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.ViewModels.Map;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.ViewModels.Settings;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels.Album {

	/// <summary>
	/// アルバムViewModel
	/// </summary>
	internal class AlbumViewModel : MediaFileCollectionViewModel<AlbumModel> {

		/// <summary>
		/// 操作受信
		/// </summary>
		public GestureReceiver GestureReceiver {
			get {
				return this.Model.GestureReceiver;
			}
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public IReadOnlyReactiveProperty<string> Title {
			get;
		}

		/// <summary>
		/// アルバム読み込み時間(ms)
		/// </summary>
		public IReadOnlyReactiveProperty<long> ResponseTime {
			get;
		}

		/// <summary>
		/// カレントインデックス番号
		/// </summary>
		public IReactiveProperty<int> CurrentIndex {
			get;
		}

		/// <summary>
		/// フィルタリング前件数
		/// </summary>
		public IReadOnlyReactiveProperty<int> BeforeFilteringCount {
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
		public IReadOnlyReactiveProperty<MediaFileInformationViewModel> MediaFileInformation {
			get;
		}

		/// <summary>
		/// 表示モード
		/// </summary>
		public IReactiveProperty<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// 表示モードに対応した型
		/// </summary>
		public IReadOnlyReactiveProperty<DisplayBase> DisplayViewModel {
			get;
		}

		/// <summary>
		/// ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		/// <summary>
		/// 表示モード変更コマンド
		/// </summary>
		public ReactiveCommand<DisplayMode> ChangeDisplayModeCommand {
			get;
		} = new ReactiveCommand<DisplayMode>();

		/// <summary>
		/// マップ
		/// </summary>
		public IReadOnlyReactiveProperty<MapViewModel> Map {
			get;
		}

		/// <summary>
		/// ファイル追加コマンド
		/// </summary>
		public ReactiveCommand<IEnumerable<IMediaFileViewModel>> AddMediaFileCommand {
			get;
		} = new ReactiveCommand<IEnumerable<IMediaFileViewModel>>();

		/// <summary>
		/// ファイル削除コマンド
		/// </summary>
		public ReactiveCommand<IEnumerable<IMediaFileViewModel>> RemoveMediaFileCommand {
			get;
		} = new ReactiveCommand<IEnumerable<IMediaFileViewModel>>();

		/// <summary>
		/// 表示する列
		/// </summary>
		public ReadOnlyReactiveCollection<Col> Columns {
			get;
		}

		public ReactiveCommand OpenColumnSettingsWindowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// このアルバムが登録アルバムか否かを示す。
		/// </summary>
		public bool IsRegisteredAlbum {
			get {
				return this.Model is RegisteredAlbum;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public AlbumViewModel(AlbumModel model) : base(model) {
			this.Title = this.Model.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ResponseTime = this.Model.ResponseTime.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.BeforeFilteringCount = this.Model.BeforeFilteringCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.Map = this.Model.Map.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ZoomLevel = this.Model.ZoomLevel.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.MediaFileInformation =
				this.Model
					.MediaFileInformation
					.Select(this.ViewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			this.DisplayMode = this.Model.DisplayMode.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			// DataTemplateのDataTypeでテンプレートを切り替えるための苦肉の策
			// どのクラスもこのアルバムViewModel(this)のインスタンスを持っていて、型だけが違う
			// これらの3つのクラスをDataTemplateのDataTypeにバインドすることでViewを切り替えている
			this.DisplayViewModel = this.DisplayMode.Select<DisplayMode, DisplayBase>(x => {
				switch (x) {
					default:
						return new DisplayLibrary(this);
					case Composition.Enum.DisplayMode.Detail:
						return new DisplayDetail(this);
					case Composition.Enum.DisplayMode.Map:
						return new DisplayMap(this);
				}
			}).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.CurrentIndex = this.Model.CurrentIndex.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);

			this.CurrentItem = this.Model.CurrentMediaFile.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.Columns = this.Settings.GeneralSettings.EnabledColumns.ToReadOnlyReactiveCollection(x => new Col(x));

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
					Get.Instance<AlbumContainer>().OnAlbumUpdated(ra.AlbumId.Value);
				}
			});

			this.RemoveMediaFileCommand.Subscribe(x => {
				if (this.Model is RegisteredAlbum ra) {

					using var vm = new DialogViewModel("確認", $"{x.Count()} 件のメディアファイルをアルバム [{ra.Title.Value}] から削除します。", MessageBoxButton.OKCancel, MessageBoxResult.Cancel);
					var message = new TransitionMessage(vm, "ShowDialog");
					this.Messenger.Raise(message);
					if (vm.Result.Value == MessageBoxResult.OK) {
						ra.RemoveFiles(x.Select(vm => vm.Model));
						Get.Instance<AlbumContainer>().OnAlbumUpdated(ra.AlbumId.Value);
					}
				}
			});

			// 表示列設定ウィンドウ
			this.OpenColumnSettingsWindowCommand.Subscribe(_ => {
				var vm = new ColumnSettingsViewModel();
				var message = new TransitionMessage(typeof(ColumnSettingsWindow), vm, TransitionMode.NewOrActive);
				this.Messenger.Raise(message);
			}).AddTo(this.CompositeDisposable);
		}
	}

	#region 苦肉の策
	internal abstract class DisplayBase {
		public AlbumViewModel AlbumViewModel {
			get;
		}

		protected DisplayBase(AlbumViewModel albumViewModel) {
			this.AlbumViewModel = albumViewModel;
		}
	}
	internal class DisplayLibrary : DisplayBase {
		public DisplayLibrary(AlbumViewModel albumViewModel) : base(albumViewModel) {
		}
	}
	internal class DisplayDetail : DisplayBase {
		public DisplayDetail(AlbumViewModel albumViewModel) : base(albumViewModel) {
		}
	}
	internal class DisplayMap : DisplayBase {
		public DisplayMap(AlbumViewModel albumViewModel) : base(albumViewModel) {
		}
	}
	#endregion

	internal class Col {
		public AvailableColumns AlternateKey {
			get;
		}

		public Col(AvailableColumns alternateKey) {
			this.AlternateKey = alternateKey;
		}
	}
}
