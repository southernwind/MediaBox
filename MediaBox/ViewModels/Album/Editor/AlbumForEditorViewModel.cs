using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Editor;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.Views.Dialog;

namespace SandBeige.MediaBox.ViewModels.Album {

	/// <summary>
	/// アルバム編集用ViewModel
	/// </summary>
	public class AlbumForEditorViewModel : ViewModelBase {
		private readonly AlbumForEditorModel _model;

		/// <summary>
		/// 操作受信
		/// </summary>
		public IGestureReceiver GestureReceiver {
			get {
				return this._model.GestureReceiver;
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

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデルインスタンス</param>
		public AlbumForEditorViewModel(AlbumForEditorModel model, IDialogService dialogService, ViewModelFactory viewModelFactory, IAlbumContainer albumContainer) {
			this._model = model;
			this.Title = this._model.Title.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.ZoomLevel = this._model.ZoomLevel.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.CurrentItem = this._model.CurrentMediaFile.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			// VM⇔Model間双方向同期
			this.SelectedMediaFiles.TwoWaySynchronize(
				this._model.CurrentMediaFiles,
				x => x.Select(vm => vm.Model).ToArray(),
				x => x.Select(viewModelFactory.Create).ToArray());

			// ファイル追加コマンド
			this.AddMediaFileCommand.Subscribe(x => {
				this._model.AddFiles(x.Select(vm => vm.Model));
				albumContainer.OnAlbumUpdated(this._model.AlbumId.Value);
			});

			this.RemoveMediaFileCommand.Subscribe(x => {
				var param = new DialogParameters() {
					{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
					{CommonDialogWindowViewModel.ParameterNameMessage ,$"{x.Count()} 件のメディアファイルをアルバム [{this.Title.Value}] から削除します。"},
					{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
					{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
				};
				dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {
					if (result.Result == ButtonResult.OK) {
						this._model.RemoveFiles(x.Select(vm => vm.Model));
						albumContainer.OnAlbumUpdated(this._model.AlbumId.Value);
					}
				});
			});
		}
	}
}
