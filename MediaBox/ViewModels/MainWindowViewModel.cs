
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.TaskQueue;
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
		/// タスクキュー
		/// </summary>
		public PriorityTaskQueue TaskQueue {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel() {
			this.NavigationMenuViewModel = Get.Instance<NavigationMenuViewModel>().AddTo(this.CompositeDisposable);
			this.AlbumSelectorViewModel = Get.Instance<AlbumSelectorViewModel>().AddTo(this.CompositeDisposable);
			this.TaskQueue = Get.Instance<PriorityTaskQueue>().AddTo(this.CompositeDisposable);
		}
	}
}
