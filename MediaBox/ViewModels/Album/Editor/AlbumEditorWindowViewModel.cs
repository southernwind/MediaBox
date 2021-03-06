using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Editor;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.ViewModels.Album.Box;
using SandBeige.MediaBox.ViewModels.Album.Selector;
using SandBeige.MediaBox.Views.Album.Box;

namespace SandBeige.MediaBox.ViewModels.Album.Editor {
	/// <summary>
	/// アルバムエディターウィンドウViewModel
	/// </summary>
	public class AlbumEditorWindowViewModel : DialogViewModelBase {
		private readonly IAlbumEditor _model;

		public enum AlbumEditorMode {
			Create,
			Edit
		};


		/// <summary>
		/// ダイアログタイトル
		/// </summary>
		public override string? Title {
			get {
				return "アルバム編集";
			}
			set {
			}
		}

		/// <summary>
		/// メディア選択用アルバムセレクター
		/// </summary>
		public AlbumSelectorViewModel AlbumSelectorViewModel {
			get;
		}

		/// <summary>
		/// 選択中未追加メディア
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedNotAddedMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// 選択中追加済みメディア
		/// </summary>
		public IReactiveProperty<IEnumerable<IMediaFileViewModel>> SelectedAddedMediaFiles {
			get;
		} = new ReactivePropertySlim<IEnumerable<IMediaFileViewModel>>(Array.Empty<IMediaFileViewModel>());

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public IReactiveProperty<int?> AlbumBoxId {
			get;
		}

		/// <summary>
		/// アルバムボックスタイトル
		/// </summary>
		public IReadOnlyReactiveProperty<string?[]> AlbumBoxTitle {
			get;
		}

		/// <summary>
		/// パス
		/// </summary>
		public IReactiveProperty<string> AlbumTitle {
			get;
		}

		/// <summary>
		/// 監視ディレクトリ
		/// </summary>
		public ReadOnlyReactiveCollection<string> MonitoringDirectories {
			get;
		}

		/// <summary>
		/// 選択中監視ディレクトリ
		/// </summary>
		public IReactiveProperty<string> SelectedMonitoringDirectory {
			get;
		} = new ReactiveProperty<string>();

		/// <summary>
		/// アルバムに監視ディレクトリを追加する
		/// </summary>
		public ReactiveCommand AddMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// アルバムから監視ディレクトリを削除する
		/// </summary>
		public ReactiveCommand RemoveMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 保存コマンド
		/// </summary>
		public ReactiveCommand SaveCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 読み込みコマンド
		/// </summary>
		public AsyncReactiveCommand LoadCommand {
			get;
		} = new AsyncReactiveCommand();
		/// <summary>
		/// アルバムボックス変更コマンド
		/// </summary>
		public ReactiveCommand AlbumBoxChangeCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumEditorWindowViewModel(
			IAlbumEditor albumEditor,
			IAlbumSelectorProvider albumSelectorProvider,
			ViewModelFactory viewModelFactory,
			IDialogService dialogService,
			IFolderSelectionDialogService folderSelectionDialogService) {
			this._model = albumEditor.AddTo(this.CompositeDisposable);
			this.ModelForToString = this._model;
			this.AlbumSelectorViewModel = viewModelFactory.Create(albumSelectorProvider.Create("editor"));

			this.AlbumBoxId = this._model.AlbumBoxId.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.AlbumBoxTitle = this._model.AlbumBoxTitle.ToReadOnlyReactivePropertySlim(null!).AddTo(this.CompositeDisposable);
			this.AlbumTitle = this._model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.MonitoringDirectories = this._model.MonitoringDirectories.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.AddMonitoringDirectoryCommand
				.Subscribe(() => {
					folderSelectionDialogService.Title = "監視するディレクトリの選択";
					if (!folderSelectionDialogService.ShowDialog() || folderSelectionDialogService.FolderName == null) {
						return;
					}
					this._model.AddDirectory(folderSelectionDialogService.FolderName);
				}).AddTo(this.CompositeDisposable);

			this.RemoveMonitoringDirectoryCommand.Subscribe(_ => this._model.RemoveDirectory(this.SelectedMonitoringDirectory.Value)).AddTo(this.CompositeDisposable);

			this.AlbumBoxChangeCommand.Subscribe(_ => {
				dialogService.ShowDialog(nameof(AlbumBoxSelectorWindow), null, result => {
					if (result.Result == ButtonResult.OK) {
						this.AlbumBoxId.Value = result.Parameters.GetValue<int>(AlbumBoxSelectorWindowViewModel.ParameterNameId);
					}
				});
			});

			this.SaveCommand.Subscribe(x => {
				this._model.Save();
				this.CloseRequest(ButtonResult.OK);
			}).AddTo(this.CompositeDisposable);

			this.LoadCommand.Subscribe(this._model.Load).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// ダイアログオープン時処理
		/// </summary>
		/// <param name="parameters"></param>
		public override void OnDialogOpened(IDialogParameters parameters) {
			if (parameters.TryGetValue<int?>(AlbumEditorModeToString(AlbumEditorMode.Create), out var id)) {
				this.AlbumBoxId.Value = id;
			} else if (parameters.TryGetValue<IEditableAlbumObject>(AlbumEditorModeToString(AlbumEditorMode.Edit), out var rao)) {
				this._model.EditAlbum(rao);
				this.LoadCommand.Execute();
			}
		}

		public static string AlbumEditorModeToString(AlbumEditorMode mode) {
			return mode switch
			{
				AlbumEditorMode.Create => "create",
				AlbumEditorMode.Edit => "edit",
				_ => throw new InvalidEnumArgumentException(),
			};
		}
	}
}
