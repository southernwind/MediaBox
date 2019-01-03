using System;

using Livet.Messaging;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

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
		/// アルバムセレクター
		/// </summary>
		public AlbumSelectorViewModel AlbumSelectorViewModel {
			get;
		}

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
			this.AlbumSelectorViewModel = Get.Instance<AlbumSelectorViewModel>().AddTo(this.CompositeDisposable);

			this.OpenAlbumCreateWindowCommand.Subscribe(_ => {
				using (var vm = Get.Instance<AlbumCreatorViewModel>()) {
					vm.CreateAlbumCommand.Execute();
					var message = new TransitionMessage(typeof(Views.SubWindows.AlbumCreateWindow.AlbumCreateWindow), vm, TransitionMode.NewOrActive);
					this.Messenger.Raise(message);
				}
			}).AddTo(this.CompositeDisposable);
		}
	}
}
