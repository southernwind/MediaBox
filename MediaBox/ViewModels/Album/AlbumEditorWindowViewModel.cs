using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;

using Livet.Messaging.IO;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Models.Album;

namespace SandBeige.MediaBox.ViewModels.Album {
	/// <summary>
	/// アルバムエディターウィンドウViewModel
	/// </summary>
	public class AlbumEditorWindowViewModel : DialogViewModelBase {
		private readonly AlbumEditor _model;

		public enum AlbumEditorMode {
			create,
			edit
		};


		/// <summary>
		/// ダイアログタイトル
		/// </summary>
		public override string Title {
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
		public IReadOnlyReactiveProperty<string[]> AlbumBoxTitle {
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
		public ReactiveCommand<FolderSelectionMessage> AddMonitoringDirectoryCommand {
			get;
		} = new ReactiveCommand<FolderSelectionMessage>();

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
		public ReactiveCommand LoadCommand {
			get;
		} = new ReactiveCommand();
		/// <summary>
		/// アルバムボックス変更コマンド
		/// </summary>
		public ReactiveCommand AlbumBoxChangeCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AlbumEditorWindowViewModel(AlbumEditor albumEditor, EditorAlbumSelectorViewModel editorAlbumSelectorViewModel, IDialogService dialogService) {
			this._model = albumEditor.AddTo(this.CompositeDisposable);
			this.ModelForToString = this._model;
			this.AlbumSelectorViewModel = editorAlbumSelectorViewModel;

			this.AlbumBoxId = this._model.AlbumBoxId.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.AlbumBoxTitle = this._model.AlbumBoxTitle.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			this.AlbumTitle = this._model.Title.ToReactivePropertyAsSynchronized(x => x.Value).AddTo(this.CompositeDisposable);
			this.MonitoringDirectories = this._model.MonitoringDirectories.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);

			this.AlbumSelectorViewModel.CurrentAlbum.Subscribe(x => {
				this.SelectedNotAddedMediaFiles.Value = Array.Empty<IMediaFileViewModel>();
			});

			this.AddMonitoringDirectoryCommand
				.Subscribe(x => {
					if (x.Response == null) {
						return;
					}
					this._model.AddDirectory(x.Response);
				}).AddTo(this.CompositeDisposable);

			this.RemoveMonitoringDirectoryCommand.Subscribe(_ => this._model.RemoveDirectory(this.SelectedMonitoringDirectory.Value)).AddTo(this.CompositeDisposable);

			this.AlbumBoxChangeCommand.Subscribe(_ => {
				dialogService.ShowDialog(nameof(Views.Album.AlbumBoxSelectorWindow), null, result => {
					if (result.Result == ButtonResult.OK) {
						this.AlbumBoxId.Value = result.Parameters.GetValue<int>(AlbumBoxSelectorViewModel.ParameterNameId);
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
			if (parameters.TryGetValue<int?>(AlbumEditorModeToString(AlbumEditorMode.create), out var id)) {
				this._model.CreateAlbum();
				this.AlbumBoxId.Value = id;
				return;
			} else if (parameters.TryGetValue<AlbumViewModel>(AlbumEditorModeToString(AlbumEditorMode.edit), out var vm)) {
				if (vm.Model is RegisteredAlbum ra) {
					this._model.EditAlbum(ra);
					this._model.Load();
				}
			}
		}

		public static string AlbumEditorModeToString(AlbumEditorMode mode) {
			switch (mode) {
				case AlbumEditorMode.create:
					return "create";
				case AlbumEditorMode.edit:
					return "edit";
				default:
					throw new InvalidEnumArgumentException();
			}
		}
	}
}
