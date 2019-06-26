using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// タスク定義
	/// </summary>
	/// <remarks>
	/// タスクが完了すると<see cref="OnTaskCompleted"/>が流れる。
	/// </remarks>
	internal class TaskAction : ModelBase, IComparable {
		private Task _task;
		/// <summary>
		/// 実行するタスク
		/// </summary>
		protected readonly Func<TaskActionState, Task> Action;

		/// <summary>
		/// タスク完了通知用サブジェクト
		/// </summary>
		protected readonly Subject<Unit> OnTaskCompletedSubject = new Subject<Unit>();

		/// <summary>
		/// エラー通知用サブジェクト
		/// </summary>
		protected readonly Subject<Exception> OnErrorSubject = new Subject<Exception>();

		/// <summary>
		/// タスク名
		/// </summary>
		public IReactiveProperty<string> TaskName {
			get;
		} = new ReactivePropertySlim<string>();

		/// <summary>
		/// 進捗最大値
		/// </summary>
		public IReactiveProperty<double?> ProgressMax {
			get;
		} = new ReactivePropertySlim<double?>();

		public IReadOnlyReactiveProperty<bool> IsIndeterminate {
			get;
		}

		/// <summary>
		/// 進捗現在値
		/// </summary>
		public IReactiveProperty<double> ProgressValue {
			get;
		} = new ReactivePropertySlim<double>();

		/// <summary>
		/// 進捗率
		/// </summary>
		public IReadOnlyReactiveProperty<double> ProgressRate {
			get;
		}

		/// <summary>
		/// タスク完了通知
		/// </summary>
		public IObservable<Unit> OnTaskCompleted {
			get {
				return this.OnTaskCompletedSubject.AsObservable();
			}
		}

		/// <summary>
		/// エラー通知
		/// </summary>
		public IObservable<Exception> OnError {
			get {
				return this.OnErrorSubject.AsObservable();
			}
		}

		/// <summary>
		/// タスク優先度
		/// </summary>
		public Priority Priority {
			get;
		}

		/// <summary>
		/// タスクキャンセル用トークンソース
		/// </summary>
		public CancellationTokenSource CancellationTokenSource {
			get;
		}

		/// <summary>
		/// タスク開始条件
		/// </summary>
		public Func<bool> TaskStartCondition {
			get;
		}

		/// <summary>
		/// タスク状態
		/// </summary>
		public TaskState TaskState {
			get;
			set;
		} = TaskState.Waiting;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="taskName">タスク名</param>
		/// <param name="action">タスク</param>
		/// <param name="priority">タスク優先度</param>
		/// <param name="token">キャンセルトークン</param>
		/// <param name="taskStartCondition">タスク開始条件</param>
		public TaskAction(string taskName, Func<TaskActionState, Task> action, Priority priority, CancellationTokenSource cancellationTokenSource, Func<bool> taskStartCondition = null) {
			this.TaskName.Value = taskName;
			this.Action = action;
			this.Priority = priority;
			this.CancellationTokenSource = cancellationTokenSource;
			this.TaskStartCondition = taskStartCondition ?? (() => true);

			this.ProgressRate =
				this.ProgressMax
					.CombineLatest(this.ProgressValue, (max, value) => (max, value))
					.Select(x => {
						if (!(x.max is { } d)) {
							return 0;
						}
						return x.value / d;
					})
					.ToReadOnlyReactivePropertySlim();
			this.IsIndeterminate = this.ProgressMax.Select(x => x == null).ToReadOnlyReactivePropertySlim();

		}

		/// <summary>
		/// バックグラウンド実行
		/// </summary>
		public void BackgroundStart() {
			this._task = this.DoAsync();
		}

		/// <summary>
		/// タスク実行予約
		/// </summary>
		public void Reserve() {
			this.TaskState = TaskState.Reserved;
		}

		public async Task DoAsync() {
			try {
				if (this.TaskState != TaskState.Reserved) {
					throw new InvalidOperationException();
				}
				this.TaskState = TaskState.WorkInProgress;
				await this.Action(new TaskActionState(this.TaskName, this.ProgressMax, this.ProgressValue, this.CancellationTokenSource.Token));
				this.TaskState = TaskState.Done;
				this.OnTaskCompletedSubject.OnNext(Unit.Default);
			} catch (Exception ex) {
				this.TaskState = TaskState.Error;
				this.OnErrorSubject.OnNext(ex);
			}
		}

		/// <summary>
		/// タスク終了待ち
		/// </summary>
		/// <returns>タスク</returns>
		public async Task Wait() {
			await this._task;
		}

		/// <summary>
		/// 比較
		/// </summary>
		/// <param name="obj">比較対象</param>
		/// <returns>比較結果</returns>
		public int CompareTo(object obj) {
			if (obj is TaskAction ta) {
				return this.Priority.CompareTo(ta.Priority);
			}
			return -1;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.TaskName}>";
		}

		protected override void Dispose(bool disposing) {
			this.CancellationTokenSource.Dispose();
			if (this.TaskState == TaskState.WorkInProgress) {
				this.OnTaskCompleted.FirstAsync();
			}
			base.Dispose(disposing);
		}
	}

	public enum TaskState {
		Waiting,
		Reserved,
		WorkInProgress,
		Done,
		Error
	}
}
