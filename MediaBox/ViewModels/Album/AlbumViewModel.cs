using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.ViewModels.Album {

	/// <summary>
	/// アルバムViewModel
	/// </summary>
	public class AlbumViewModel : MediaFileCollectionViewModel<AlbumModel>, IAlbumViewModel {
		public IAlbumModel AlbumModel {
			get {
				return this.Model;
			}
		}

		/// <summary>
		/// 操作受信
		/// </summary>
		public IGestureReceiver GestureReceiver {
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
		/// ズームレベル
		/// </summary>
		public IReadOnlyReactiveProperty<int> ZoomLevel {
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

		public ReadOnlyReactiveCollection<IAlbumViewerViewViewModelPair> AlbumViewers {
			get {
				return this.Model.AlbumViewers;
			}
		}

		public IReactiveProperty<IAlbumViewerViewViewModelPair> CurrentAlbumViewer {
			get {
				return this.Model.CurrentAlbumViewer;
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public AlbumViewModel(AlbumModel model, ViewModelFactory viewModelFactory) : base(model, viewModelFactory) {
			this.Title = this.Model.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ResponseTime = this.Model.ResponseTime.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.BeforeFilteringCount = this.Model.BeforeFilteringCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ZoomLevel = this.Model.ZoomLevel.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.CurrentItem = this.Model.CurrentMediaFile.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// VM⇔Model間双方向同期
			this.SelectedMediaFiles.TwoWaySynchronize(
				this.Model.CurrentMediaFiles,
				x => x.Select(vm => vm.Model).ToArray(),
				x => x.Select(viewModelFactory.Create).ToArray());
		}
	}
}
