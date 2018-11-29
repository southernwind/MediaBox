using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

using Livet;
using Livet.Commands;
using Livet.Messaging;
using Livet.Messaging.IO;
using Livet.EventListeners;
using Livet.Messaging.Windows;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.ViewModels.Media;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Base;
using Reactive.Bindings;
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow;
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow.Pages;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// メインウィンドウViewModel
	/// </summary>
	internal class MainWindowViewModel : ViewModelBase {
		/// <summary>
		/// ナビゲーションメニューViewModel
		/// </summary>
		public NavigationMenuViewModel NavigationMenuViewModel {
			get;
		}

		/// <summary>
		/// アルバムコンテナ
		/// </summary>
		public AlbumContainerViewModel AlbumContainerViewModel {
			get;
		}

		/// <summary>
		/// 利用中のViewModel
		/// </summary>
		public ReactiveProperty<ViewModelBase> CurrentContentViewModel {
			get;
		} = new ReactiveProperty<ViewModelBase>();

		/// <summary>
		/// ディレクトリドロップ
		/// </summary>
		public ReactiveCommand<IEnumerable<string>> DirectoryDragAndDropCommand {
			get;
		} = new ReactiveCommand<IEnumerable<string>>();

		/// <summary>
		/// アルバム作成に切り替えるコマンド
		/// </summary>
		public ReactiveCommand OpenAlbumCreatorCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// メディアライブラリ表示に切り替えるコマンド
		/// </summary>
		public ReactiveCommand OpenMediaLibraryCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel() {
			this.NavigationMenuViewModel = Get.Instance<NavigationMenuViewModel>().AddTo(this.CompositeDisposable);
			this.AlbumContainerViewModel = Get.Instance<AlbumContainerViewModel>().AddTo(this.CompositeDisposable);
			this.CurrentContentViewModel.Value = this.AlbumContainerViewModel;

			this.OpenAlbumCreatorCommand.Subscribe(_ => {
				this.CurrentContentViewModel.Value = Get.Instance<AlbumCreatorViewModel>();
			});

			this.OpenMediaLibraryCommand.Subscribe(_ => {
				this.CurrentContentViewModel.Value = this.AlbumContainerViewModel;
			});

			// ディレクトリドロップ
			this.DirectoryDragAndDropCommand.Subscribe(x => {
				using (var vm = Get.Instance<OptionWindowViewModel>()) {
					var pathSettingsPage = vm.ChangeCurrentPage<PathSettingsViewModel>();
					pathSettingsPage.AddMonioringDirectory(x);
					var message = new TransitionMessage(typeof(Views.SubWindows.OptionWindow.OptionWindow), vm, TransitionMode.Modal);
					this.Settings.Save();
					this.Messenger.Raise(message);
				}
			});
		}

		/// <summary>
		/// 初期処理
		/// </summary>
		public void Initialize() {
		}
	}
}
