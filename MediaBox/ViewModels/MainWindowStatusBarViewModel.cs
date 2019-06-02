using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels {
	internal class MainWindowStatusBarViewModel : ViewModelBase {

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

		public ReadOnlyReactiveCollection<TaskAction> ProgressingTaskList {
			get;
		}

		public ReadOnlyReactivePropertySlim<int> TaskCount {
			get;
		}

		public MainWindowStatusBarViewModel() {
			this.TaskQueue = Get.Instance<PriorityTaskQueue>();
			this.TaskQueueListShowCommand.Subscribe(() => {
				this.TaskQueueListVisibility.Value = true;
			});
			lock (this.TaskQueue.ProgressingTaskList) {
				this.ProgressingTaskList = this.TaskQueue.ProgressingTaskList.ToReadOnlyReactiveCollection().AddTo(this.CompositeDisposable);
			}
			this.TaskCount = this.TaskQueue.TaskCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
			;
		}
	}
}
