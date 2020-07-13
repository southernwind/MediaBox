
using System.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
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
		/// 初期処理コマンド
		/// </summary>
		public ReactiveCommand InitializeCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel(AlbumSelector albumSelector) {
			albumSelector.SetName("main");
			albumSelector.SetAlbumToCurrent(this.States.AlbumStates.AlbumHistory.Value.FirstOrDefault());
			this.AlbumSelectorViewModel = new AlbumSelectorViewModel(albumSelector).AddTo(this.CompositeDisposable);
			this.NavigationMenuViewModel = new NavigationMenuViewModel(albumSelector).AddTo(this.CompositeDisposable);
			Get.Instance<MediaFileManager>();

			this.InitializeCommand.Subscribe(() => {
				this.Logging.Log("起動完了");
			});
		}
	}
}
