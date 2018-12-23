using System;
using System.Reactive.Linq;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Library.Extensions;
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
		/// カレントメディアファイル
		/// </summary>
		public ReadOnlyReactivePropertySlim<MediaFileViewModel> CurrentItem {
			get;
		}

		/// <summary>
		/// 複数メディアファイルまとめてプロパティ表示用ViewModel
		/// </summary>
		public ReadOnlyReactivePropertySlim<MediaFilePropertiesViewModel> MediaFileProperties {
			get;
		}

		/// <summary>
		/// 表示モード
		/// </summary>
		public ReadOnlyReactivePropertySlim<DisplayMode> DisplayMode {
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

			this.MediaFileProperties = this.Model.MediaFileProperties.Select(x => Get.Instance<MediaFilePropertiesViewModel>(x)).ToReadOnlyReactivePropertySlim();

			this.DisplayMode = this.Model.DisplayMode.ToReadOnlyReactivePropertySlim();

			this.CurrentItem = this.Model.CurrentMediaFile.Select(x => x == null ? null : Get.Instance<MediaFileViewModel>(x)).ToReadOnlyReactivePropertySlim();

			// 選択アイテム(複数)のViewModel→Model片方向同期
			this.SelectedMediaFiles.SynchronizeTo(this.Model.CurrentMediaFiles, x => x.Model).AddTo(this.CompositeDisposable);
			
			// 表示モード変更コマンド
			this.ChangeDisplayModeCommand.Subscribe(this.Model.ChangeDisplayMode);
		}
	}
}
