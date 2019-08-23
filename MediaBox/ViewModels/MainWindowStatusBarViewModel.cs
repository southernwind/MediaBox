using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Models.Notification;
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

		/// <summary>
		/// 処理中タスクリスト
		/// </summary>
		public ReadOnlyReactiveCollection<TaskAction> ProgressingTaskList {
			get;
		}

		/// <summary>
		/// 残りタスク件数
		/// </summary>
		public ReadOnlyReactivePropertySlim<int> TaskCount {
			get;
		}

		/// <summary>
		/// 通知を表示するか否か
		/// </summary>
		public IReadOnlyReactiveProperty<bool> NotificationVisible {
			get;
		}

		/// <summary>
		/// 通知内容
		/// </summary>
		public IReactiveProperty<INotification> Notification {
			get;
		} = new ReactivePropertySlim<INotification>();

		public MainWindowStatusBarViewModel() {
			this.TaskQueue = Get.Instance<PriorityTaskQueue>();
			this.TaskQueueListShowCommand.Subscribe(() => {
				this.TaskQueueListVisibility.Value = true;
			});
			lock (this.TaskQueue.ProgressingTaskList) {
				this.ProgressingTaskList = this.TaskQueue.ProgressingTaskList.ToReadOnlyReactiveCollection(disposeElement: false).AddTo(this.CompositeDisposable);
			}
			this.TaskCount = this.TaskQueue.TaskCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			var notificationManager = Get.Instance<NotificationManager>();
			notificationManager.OnNotify.Subscribe(x => {
				this.Notification.Value = x;
			}).AddTo(this.CompositeDisposable);

			this.Notification
				.Throttle(TimeSpan.FromSeconds(5))
				.Subscribe(_ => {
					this.Notification.Value = null;
				}).AddTo(this.CompositeDisposable);
			this.NotificationVisible = this.Notification.Select(x => x != null).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);
		}
	}
}
