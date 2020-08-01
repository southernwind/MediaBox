using System;

using Livet.Messaging.IO;

using Prism.Services.Dialogs;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Selector;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Views.About;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// ナビゲーションウィンドウViewModel
	/// </summary>
	public class NavigationMenuViewModel : ViewModelBase {
		/// <summary>
		/// ファイル追加コマンド
		/// </summary>
		public ReactiveCommand<OpeningFileSelectionMessage> AddFileCommand {
			get;
		} = new ReactiveCommand<OpeningFileSelectionMessage>();

		/// <summary>
		/// フォルダ追加コマンド
		/// </summary>
		public ReactiveCommand<FolderSelectionMessage> AddFolderCommand {
			get;
		} = new ReactiveCommand<FolderSelectionMessage>();

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
		public ReactiveCommand<IAlbumObject> SetCurrentAlbumCommand {
			get;
		} = new ReactiveCommand<IAlbumObject>();
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NavigationMenuViewModel(MainAlbumSelector albumSelector, IDialogService dialogService, ISettings settings, MediaFileManager mediaFileManager) {
			this.AddFileCommand.Subscribe(x => {
				if (x.Response == null) {
					return;
				}
				mediaFileManager.RegisterItems(x.Response);
			});

			this.AddFolderCommand.Subscribe(x => {
				if (x.Response == null) {
					return;
				}
				mediaFileManager.RegisterItems(x.Response);
			});

			this.SettingsWindowOpenCommand.Subscribe(() => {
				settings.Save();
				dialogService.Show(nameof(SettingsWindow), null, null);
			}).AddTo(this.CompositeDisposable);

			this.AboutWindowOpenCommand.Subscribe(() => {
				dialogService.Show(nameof(AboutWindow), null, null);

			}).AddTo(this.CompositeDisposable);

			this.SetCurrentAlbumCommand.Subscribe(albumSelector.SetAlbumToCurrent).AddTo(this.CompositeDisposable);
		}
	}
}
