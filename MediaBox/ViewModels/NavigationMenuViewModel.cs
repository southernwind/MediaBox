using System;

using Livet.Messaging;
using Livet.Messaging.IO;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.About;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.History.Creator;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.About;
using SandBeige.MediaBox.ViewModels.Settings;
using SandBeige.MediaBox.Views;
using SandBeige.MediaBox.Views.Settings;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// ナビゲーションウィンドウViewModel
	/// </summary>
	internal class NavigationMenuViewModel : ViewModelBase {
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

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public NavigationMenuViewModel(AlbumSelector albumSelector) {
			this.AddFileCommand.Subscribe(x => {
				Get.Instance<MediaFileManager>().RegisterItems(x.Response);
			});

			this.AddFolderCommand.Subscribe(x => {
				Get.Instance<MediaFileManager>().RegisterItems(x.Response);
			});

			this.SettingsWindowOpenCommand.Subscribe(() => {
				var vm = new SettingsWindowViewModel();
				var message = new TransitionMessage(typeof(SettingsWindow), vm, TransitionMode.NewOrActive);
				this.Settings.Save();
				this.Messenger.Raise(message);
			}).AddTo(this.CompositeDisposable);

			this.AboutWindowOpenCommand.Subscribe(() => {
				using var model = new AboutModel();
				var vm = new AboutWindowViewModel(model);
				var message = new TransitionMessage(typeof(AboutWindow), vm, TransitionMode.NewOrActive);
				this.Messenger.Raise(message);

			}).AddTo(this.CompositeDisposable);

			this.SetCurrentAlbumCommand.Subscribe(albumSelector.SetAlbumToCurrent).AddTo(this.CompositeDisposable);
		}

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
		public ReactiveCommand<IAlbumCreator> SetCurrentAlbumCommand {
			get;
		} = new ReactiveCommand<IAlbumCreator>();
		#endregion
	}
}
