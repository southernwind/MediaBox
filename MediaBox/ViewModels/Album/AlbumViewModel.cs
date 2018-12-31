using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Library.Extensions;
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

			this.MonitoringDirectories = this.Model.MonitoringDirectories.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.Map = this.Model.Map.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.MediaFileProperties =
				this.Model
					.MediaFileProperties
					.Select(this.ViewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			this.DisplayMode = this.Model.DisplayMode.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.CurrentItem =
				this.Model
					.CurrentMediaFile
					.Select(x => x == null ? null : this.ViewModelFactory.Create(x))
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			// 選択アイテム(複数)のViewModel→Model片方向同期
			this.SelectedMediaFiles
				.SynchronizeTo(this.Model.CurrentMediaFiles, x => x.Model)
				.AddTo(this.CompositeDisposable);

			// 表示モード変更コマンド
			this.ChangeDisplayModeCommand
				.Subscribe(this.Model.ChangeDisplayMode)
				.AddTo(this.CompositeDisposable);
		}
	}
}
