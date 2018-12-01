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
		/// アルバム作成画面を開くコマンド
		/// </summary>
		public ReactiveCommand OpenAlbumCreateWindowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel() {
			this.NavigationMenuViewModel = Get.Instance<NavigationMenuViewModel>().AddTo(this.CompositeDisposable);
			this.AlbumContainerViewModel = Get.Instance<AlbumContainerViewModel>().AddTo(this.CompositeDisposable);

			this.OpenAlbumCreateWindowCommand.Subscribe(_ => {
				using (var vm = Get.Instance<AlbumCreatorViewModel>()) {
					vm.CreateAlbumCommand.Execute();
					var message = new TransitionMessage(typeof(Views.SubWindows.AlbumCreateWindow.AlbumCreateWindow), vm, TransitionMode.NewOrActive);
					this.Messenger.Raise(message);
				}
			});
		}
	}
}
