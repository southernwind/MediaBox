using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// タスク定義
	/// </summary>
	/// <remarks>
	/// タスクが完了すると<see cref="OnTaskCompleted"/>が流れる。
	/// </remarks>
	internal class TaskAction : ModelBase, IComparable {
		private string _taskName;
		private double? _progressMax;
		private double _progressValue;
		/// <summary>
		/// 実行するタスク
		/// </summary>
		private readonly Func<Task> _action;

		/// <summary>
		/// タスク完了通知用サブジェクト
		/// </summary>
		private readonly Subject<Unit> _onTaskCompletedSubject = new Subject<Unit>();

		/// <summary>
		/// エラー通知用サブジェクト
		/// </summary>
		private readonly Subject<Exception> _onErrorSubject = new Subject<Exception>();

		/// <summary>
		/// タスク名
		/// </summary>
		public string TaskName {
			get {
				return this._taskName;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._taskName, value);
			}
		}

		/// <summary>
		/// 進捗最大値
		/// </summary>
		public double? ProgressMax {
			get {
				return this._progressMax;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._progressMax, value, nameof(this.ProgressRate), nameof(this.IsIndeterminate));
			}
		}

		public bool IsIndeterminate {
			get {
				return this.ProgressMax == null;
			}
		}

		/// <summary>
		/// 進捗現在値
		/// </summary>
		public double ProgressValue {
			get {
				return this._progressValue;
			}
			set {
				this.RaisePropertyChangedIfSet(ref this._progressValue, value, nameof(this.ProgressRate));
			}
		}

		/// <summary>
		/// 進捗率
		/// </summary>
		public double ProgressRate {
			get {
				if (!(this.ProgressMax is { } d)) {
					return 0;
				}
				return this.ProgressValue / d;
			}
		}

		/// <summary>
		/// タスク完了通知
		/// </summary>
		public IObservable<Unit> OnTaskCompleted {
			get {
				return this._onTaskCompletedSubject.AsObservable();
			}
		}

		/// <summary>
		/// エラー通知
		/// </summary>
		public IObservable<Exception> OnErrorSubject {
			get {
				return this._onErrorSubject.AsObservable();
			}
		}

		/// <summary>
		/// タスク優先度
		/// </summary>
		public Priority Priority {
			get;
		}

		/// <summary>
		/// タスクキャンセル用トークン
		/// </summary>
		public CancellationToken Token {
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
		public TaskAction(string taskName, Func<Task> action, Priority priority, CancellationToken token, Func<bool> taskStartCondition = null) {
			this.TaskName = taskName;
			this._action = action;
			this.Priority = priority;
			this.Token = token;
			this.TaskStartCondition = taskStartCondition ?? (() => true);
		}

		/// <summary>
		/// バックグラウンド実行
		/// </summary>
		public void BackgroundStart() {
			this.DoAsync();
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
				await this._action();
				this.TaskState = TaskState.Done;
				this._onTaskCompletedSubject.OnNext(Unit.Default);
			} catch (Exception ex) {
				this.TaskState = TaskState.Error;
				this._onErrorSubject.OnNext(ex);
			}
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
	}

	public enum TaskState {
		Waiting,
		Reserved,
		WorkInProgress,
		Done,
		Error
	}
}
