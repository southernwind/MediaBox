
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
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;
using SandBeige.MediaBox.Composition.Interfaces.ViewModels.Tools;
using SandBeige.MediaBox.Models.ContextMenu;
using SandBeige.MediaBox.ViewModels.Album.Box;
using SandBeige.MediaBox.ViewModels.Dialog;
using SandBeige.MediaBox.ViewModels.Media;
using SandBeige.MediaBox.ViewModels.Media.ThumbnailCreator;
using SandBeige.MediaBox.ViewModels.Tools;
using SandBeige.MediaBox.Views.Dialog;
using SandBeige.MediaBox.Views.Media.ThumbnailCreator;

namespace SandBeige.MediaBox.ViewModels.ContextMenu {

	public class MediaFileListContextMenuViewModel : ViewModelBase, IMediaFileListContextMenuViewModel {
		private readonly MediaFileListContextMenuModel _model;

		/// <summary>
		/// 対象外部ツール
		/// </summary>
		public ReadOnlyReactiveCollection<IExternalToolViewModel> ExternalTools {
			get {
				return
					this._model
						.ExternalTools
						.ToReadOnlyReactiveCollection(x => (IExternalToolViewModel)new ExternalToolViewModel(x));
			}
		}

		/// <summary>
		/// 対象メディアファイルViewModelリスト
		/// </summary>
		public IReadOnlyReactiveProperty<IEnumerable<IMediaFileViewModel>> TargetFiles {
			get;
		}

		public IMediaFileViewModel TargetFile {
			get {
				return this.TargetFiles.Value.First();
			}
		}

		/// <summary>
		/// 階層表示用アルバム格納棚
		/// </summary>
		public IReadOnlyReactiveProperty<AlbumBoxViewModel> Shelf {
			get;
		}

		public IReadOnlyReactiveProperty<bool> IsRegisteredAlbum {
			get;
		}

		/// <summary>
		/// 評価更新コマンド
		/// </summary>
		public ReactiveCommand<int> SetRateCommand {
			get;
		} = new ReactiveCommand<int>();

		/// <summary>
		/// サムネイル再作成コマンド
		/// </summary>
		public ReactiveCommand RecreateThumbnailCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// サムネイル作成ウィンドウの起動コマンド
		/// </summary>
		public ReactiveCommand CreateVideoThumbnailWithSpecificSceneCommand {
			get;
		}

		/// <summary>
		/// ディレクトリを開く
		/// </summary>
		public ReactiveCommand OpenDirectoryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 登録から削除コマンド
		/// </summary>
		public ReactiveCommand DeleteFileFromRegistryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// アルバムからファイル削除コマンド
		/// </summary>
		public ReactiveCommand RemoveMediaFileFromAlbumCommand {
			get;
		} = new();

		/// <summary>
		/// ファイル追加コマンド
		/// </summary>
		public ReactiveCommand<int> AddMediaFileToOtherAlbumCommand {
			get;
		} = new ReactiveCommand<int>();


		public MediaFileListContextMenuViewModel(MediaFileListContextMenuModel model, IDialogService dialogService, ViewModelFactory viewModelFactory) {
			this._model = model;

			this.TargetFiles =
				model.TargetFiles
					.Select(x => x.Select(viewModelFactory.Create))
					.ToReadOnlyReactivePropertySlim()
					.AddTo(this.CompositeDisposable)!;

			this.Shelf = model.Shelf.Select(viewModelFactory.Create).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable)!;

			this.IsRegisteredAlbum = model.IsRegisteredAlbum.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.SetRateCommand.Subscribe(model.SetRate);

			this.RecreateThumbnailCommand.Subscribe(x => model.CreateThumbnail());

			this.CreateVideoThumbnailWithSpecificSceneCommand = this.TargetFiles.Select(x => x.Any(m => m is VideoFileViewModel)).ToReactiveCommand();

			this.CreateVideoThumbnailWithSpecificSceneCommand.Subscribe(_ => {
				var param = new DialogParameters {
					{
						ThumbnailCreatorWindowViewModel.ParameterNameFiles,
						this.TargetFiles.Value.OfType<VideoFileViewModel>()
					}
				};
				dialogService.Show(nameof(ThumbnailCreatorWindow), param, null);
			});
			this.OpenDirectoryCommand.Subscribe(model.OpenDirectory).AddTo(this.CompositeDisposable);

			this.DeleteFileFromRegistryCommand.Subscribe(_ => {
				var param = new DialogParameters {
					{CommonDialogWindowViewModel.ParameterNameTitle ,"確認" },
					{CommonDialogWindowViewModel.ParameterNameMessage ,$"{this.TargetFiles.Value.Count()} 件のメディアファイルを登録からを削除します。\n(実ファイルは削除されません。)" },
					{CommonDialogWindowViewModel.ParameterNameButton ,MessageBoxButton.OKCancel },
					{CommonDialogWindowViewModel.ParameterNameDefaultButton ,MessageBoxResult.Cancel},
				};
				dialogService.ShowDialog(nameof(CommonDialogWindow), param, result => {

					if (result.Result == ButtonResult.OK) {
						model.DeleteFileFromRegistry();
					}
				});
			}).AddTo(this.CompositeDisposable);

			this.RemoveMediaFileFromAlbumCommand.Subscribe(this._model.RemoveMediaFileFromAlbum).AddTo(this.CompositeDisposable);

			this.AddMediaFileToOtherAlbumCommand.Subscribe(this._model.AddMediaFileToOtherAlbum).AddTo(this.CompositeDisposable);
		}

		public void SetTargetFiles(IEnumerable<IMediaFileViewModel> targetFiles) {
			this._model.TargetFiles.Value = targetFiles.Select(x => x.Model);
		}

		public void SetTargetAlbum(IAlbumObject albumObject) {
			this._model.TargetAlbum.Value = albumObject;
		}
	}
}
