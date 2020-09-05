using System;
using System.Reactive.Linq;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Models.Notification;
namespace SandBeige.MediaBox.ViewModels {
	public class MainWindowStatusBarViewModel : ViewModelBase {

		/// <summary>
		/// タスクキュー詳細表示/非表示
		/// </summary>
		public IReactiveProperty<bool> TaskQueueListVisibility {
			get;
		} = new ReactiveProperty<bool>();

		/// <summary>
		/// タスクキュー
		/// </summary>
		public IPriorityTaskQueue TaskQueue {
			get;
		}

		/// <summary>
		/// 処理中タスクリスト
		/// </summary>
		public ReadOnlyReactiveCollection<ITaskAction> ProgressingTaskList {
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
		/// 通知詳細表示/非表示
		/// </summary>
		public IReactiveProperty<bool> NotificationVisibility {
			get;
		} = new ReactiveProperty<bool>();

		/// <summary>
		/// 通知内容
		/// </summary>
		public IReactiveProperty<INotification?> Notification {
			get;
		} = new ReactivePropertySlim<INotification?>();

		/// <summary>
		/// 未読エラーがあるかどうか
		/// </summary>
		public IReactiveProperty<bool> HasUnreadError {
			get;
		} = new ReactivePropertySlim<bool>();

		public MainWindowStatusBarViewModel(IPriorityTaskQueue taskQueue, INotificationManager notificationManager) {
			this.TaskQueue = taskQueue;
			lock (this.TaskQueue.ProgressingTaskList) {
				this.ProgressingTaskList = this.TaskQueue.ProgressingTaskList.ToReadOnlyReactiveCollection(disposeElement: false).AddTo(this.CompositeDisposable);
			}
			this.TaskCount = this.TaskQueue.TaskCount.ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			notificationManager.OnNotify.Subscribe(x => {
				if (x is Error && !this.NotificationVisibility.Value) {
					this.HasUnreadError.Value = true;
				}
				this.Notification.Value = x;
			}).AddTo(this.CompositeDisposable);

			this.Notification
				.Throttle(TimeSpan.FromSeconds(5))
				.Subscribe(_ => {
					this.Notification.Value = null;
				}).AddTo(this.CompositeDisposable);
			this.NotificationVisible = this.Notification.Select(x => x != null).ToReadOnlyReactivePropertySlim().AddTo(this.CompositeDisposable);

			this.NotificationVisibility.Where(x => x).Subscribe(x => {
				this.HasUnreadError.Value = false;
			});
		}
	}
}
