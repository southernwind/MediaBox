using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.Views.Dialog;

namespace SandBeige.MediaBox.ViewModels.Album {

	/// <summary>
	/// アルバムViewModel
	/// </summary>
	internal class AlbumViewModel : MediaFileCollectionViewModel<AlbumModel>, IAlbumViewModel {
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
		/// 複数メディアファイルまとめて情報表示用ViewModel
		/// </summary>
		public IReadOnlyReactiveProperty<MediaFileInformationViewModel> MediaFileInformation {
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

		/// <summary>
		/// このアルバムが登録アルバムか否かを示す。
		/// </summary>
		public bool IsRegisteredAlbum {
			get {
				return this.Model is RegisteredAlbum;
			}
		}

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
		public AlbumViewModel(AlbumModel model, IDialogService dialogService) : base(model) {
			this.Title = this.Model.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ResponseTime = this.Model.ResponseTime.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.BeforeFilteringCount = this.Model.BeforeFilteringCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ZoomLevel = this.Model.ZoomLevel.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.MediaFileInformation =
				this.Model
					.MediaFileInformation
					.Select(this.ViewModelFactory.Create)
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable);

			this.CurrentItem = this.Model.CurrentMediaFile.Select(this.ViewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// VM⇔Model間双方向同期
			this.SelectedMediaFiles.TwoWaySynchronize(
				this.Model.CurrentMediaFiles,
				x => x.Select(vm => vm.Model).ToArray(),
				x => x.Select(this.ViewModelFactory.Create).ToArray());

			// ファイル追加コマンド
			this.AddMediaFileCommand.Subscribe(x => {
				if (this.Model is RegisteredAlbum ra) {
					ra.AddFiles(x.Select(vm => vm.Model));
					Get.Instance<AlbumContainer>().OnAlbumUpdated(ra.AlbumId.Value);
				}
			});

			this.RemoveMediaFileCommand.Subscribe(x => {
				if (this.Model is RegisteredAlbum ra) {
					var param = new DialogParameters() {
						{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
						{CommonDialogWindowViewModel.ParameterNameMessage ,$"{x.Count()} 件のメディアファイルをアルバム [{ra.Title.Value}] から削除します。"},
						{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
						{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
					};
					dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {
						if (result.Result == ButtonResult.OK) {
							ra.RemoveFiles(x.Select(vm => vm.Model));
							Get.Instance<AlbumContainer>().OnAlbumUpdated(ra.AlbumId.Value);
						}
					});
				}
			});
		}
	}
}
