using System;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.History;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Views.About;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// ナビゲーションウィンドウViewModel
	/// </summary>
	public class NavigationMenuViewModel : ViewModelBase {
		public ReadOnlyReactiveCollection<IHistoryObject> History {
			get;
		}

		/// <summary>
		/// ファイル追加コマンド
		/// </summary>
		public ReactiveCommand AddFileCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// フォルダ追加コマンド
		/// </summary>
		public ReactiveCommand AddFolderCommand {
			get;
		} = new ReactiveCommand();

		#region WindowOpenCommand

		#region SettingsWindowOpenCommand
		/// <summary>
		/// 設定ウィンドウオープンコマンド
		/// </summary>
		public ReactiveCommand SettingsWindowOpenCommand {
			get;
		} = new ReactiveCommand();

		#endregion

		/// <summary>
		/// 概要ウィンドウオープンコマンド
		/// </summary>
		public ReactiveCommand AboutWindowOpenCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// カレントアルバム変更コマンド
		/// </summary>
		public ReactiveCommand<IHistoryObject> SetCurrentAlbumCommand {
			get;
		} = new ReactiveCommand<IHistoryObject>();
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NavigationMenuViewModel(
			IAlbumSelectorProvider albumSelectorProvider,
			IDialogService dialogService,
			ISettings settings,
			IMediaFileManager mediaFileManager,
			IStates states,
			IFolderSelectionDialogService folderSelectionDialogService,
			IOpenFileDialogService openFileDialogService) {
			this.AddFileCommand.Subscribe(() => {
				openFileDialogService.Title = "追加するファイルの選択";
				openFileDialogService.MultiSelect = true;
				if (!openFileDialogService.ShowDialog() || openFileDialogService.FileNames == null) {
					return;
				}
				mediaFileManager.RegisterItems(openFileDialogService.FileNames);
			});

			this.AddFolderCommand.Subscribe(x => {
				folderSelectionDialogService.Title = "追加するフォルダの選択";
				if (!folderSelectionDialogService.ShowDialog() || folderSelectionDialogService.FolderName == null) {
					return;
				}
				mediaFileManager.RegisterItems(folderSelectionDialogService.FolderName);
			});

			this.SettingsWindowOpenCommand.Subscribe(() => {
				settings.Save();
				dialogService.Show(nameof(SettingsWindow), null, null);
			}).AddTo(this.CompositeDisposable);

			this.AboutWindowOpenCommand.Subscribe(() => {
				dialogService.Show(nameof(AboutWindow), null, null);

			}).AddTo(this.CompositeDisposable);

			var albumSelector = albumSelectorProvider.Create("main");
			this.SetCurrentAlbumCommand.Subscribe(x => albumSelector.SetAlbumToCurrent(x.AlbumObject)).AddTo(this.CompositeDisposable);
			this.History = states.AlbumStates.AlbumHistory.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
		}
	}
}
