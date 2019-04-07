
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Album.Filter;

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
		/// タスクキュー詳細表示/非表示
		/// </summary>
		public IReactiveProperty<bool> TaskQueueListVisibility {
			get;
		} = new ReactiveProperty<bool>();

		/// <summary>
		/// タスクキュー
		/// </summary>
		public PriorityTaskQueue TaskQueue {
			get;
		}

		/// <summary>
		/// タスクキュー詳細表示コマンド
		/// </summary>
		public ReactiveCommand TaskQueueListShowCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// 初期処理コマンド
		/// </summary>
		public ReactiveCommand InitializeCommand {
			get;
		} = new ReactiveCommand();

		/// <summary>
		/// フィルター制御
		/// </summary>
		public FilterDescriptionManagerViewModel FilterDescriptionManager {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MainWindowViewModel() {
			this.AlbumSelectorViewModel = Get.InstanceWithName<AlbumSelectorViewModel>("main").AddTo(this.CompositeDisposable);
			this.NavigationMenuViewModel = Get.Instance<NavigationMenuViewModel>(this.AlbumSelectorViewModel.Model).AddTo(this.CompositeDisposable);
			this.FilterDescriptionManager = Get.Instance<FilterDescriptionManagerViewModel>().AddTo(this.CompositeDisposable);
			this.TaskQueue = Get.Instance<PriorityTaskQueue>().AddTo(this.CompositeDisposable);
			this.TaskQueueListShowCommand.Subscribe(() => {
				this.TaskQueueListVisibility.Value = true;
			});

			Get.Instance<MediaFileManager>();

			this.InitializeCommand.Subscribe(() => {
				this.Logging.Log("起動完了");
			});
			this.TaskQueue.TaskStart();
		}
	}
}
