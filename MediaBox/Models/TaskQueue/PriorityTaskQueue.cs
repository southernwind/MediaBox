using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.Composition.Logging;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// 優先度付きタスク処理機構
	/// </summary>
	/// <remarks>
	/// DIコンテナによってシングルトンとして管理され、優先度の高いものから順に処理をしていく。
	/// </remarks>
	public class PriorityTaskQueue : ModelBase, IPriorityTaskQueue {
		private bool _hasTask;
		private readonly object _hasTaskLockObj = new object();
		/// <summary>
		/// タスク状態変化通知
		/// </summary>
		private readonly Subject<Unit> _taskStateChanged = new Subject<Unit>();

		/// <summary>
		/// 今あるタスクが全部完了した通知用Subject
		/// </summary>
		private readonly Subject<Unit> _allTaskCompletedSubject = new Subject<Unit>();
		/// <summary>
		/// 今あるタスクが全部完了した通知
		/// </summary>
		public IObservable<Unit> AllTaskCompleted {
			get {
				return this._allTaskCompletedSubject.AsObservable();
			}
		}

		/// <summary>
		/// 実行待ちタスクリスト
		/// </summary>
		/// <remarks>
		/// 追加、削除はLock必須。
		/// </remarks>
		private readonly Dictionary<Priority, ObservableSynchronizedCollection<ITaskAction>> _taskList = new Dictionary<Priority, ObservableSynchronizedCollection<ITaskAction>>();

		/// <summary>
		/// 処理実行中のタスクリスト
		/// </summary>
		public ReactiveCollection<ITaskAction> ProgressingTaskList {
			get;
		} = new ReactiveCollection<ITaskAction>();

		/// <summary>
		/// タスク件数
		/// </summary>
		public IReactiveProperty<int> TaskCount {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PriorityTaskQueue(ILogging logging) {
			// TODO : デッドロック多発しそう。よく考える。できるならロックをへらす。
			var taskListChanged = new Subject<Unit>();
			foreach (var p in Enum.GetValues(typeof(Priority)).OfType<Priority>().OrderBy(x => x)) {
				var osc = new ObservableSynchronizedCollection<ITaskAction>();
				this._taskList.Add(p, osc);
				osc.CollectionChangedAsObservable().Subscribe(_ => taskListChanged.OnNext(Unit.Default));
			}

			this.ProgressingTaskList.CollectionChangedAsObservable().ToUnit()
				.Merge(taskListChanged)
				.Merge(this._taskStateChanged)
				.Subscribe(_ => {
					// タスク件数の更新
					lock (this.ProgressingTaskList) {
						this.TaskCount.Value =
							this._taskList
								.SelectMany(x => x.Value)
								.Union(this.ProgressingTaskList)
								.Count(x => x.TaskState != TaskState.Done);
					}
				});
			// 新たにタスクが追加されたり、実行中タスクが完了したタイミングで新しいタスクを実行するかを検討する。
			this.ProgressingTaskList.CollectionChangedAsObservable().ToUnit()
				.Merge(taskListChanged)
				.Merge(this._taskStateChanged)
				.Subscribe(_ => {
					Task.Run(() => {
						using (this.DisposeLock.DisposableEnterReadLock()) {
							if (this.DisposeState != DisposeState.NotDisposed) {
								return;
							}
							if (this.TaskCount.Value == 0) {
								lock (this._hasTaskLockObj) {
									if (this._hasTask) {
										this._hasTask = false;
										this._allTaskCompletedSubject.OnNext(Unit.Default);
									}
								}
								return;
							}
							if (this.ProgressingTaskList.Count > 5) {
								return;
							}
							ITaskAction ta;
							lock (this._taskList) {
								ta =
									this
										._taskList
										.SelectMany(x => x.Value)
										.FirstOrDefault(x => x.TaskStartCondition() && x.TaskState == TaskState.Waiting);
								ta?.Reserve();
							}

							if (ta != null) {
								lock (this.ProgressingTaskList) {
									this.ProgressingTaskList.Add(ta);
								}
								ta.OnTaskCompleted.Subscribe(__ => {
									lock (this.ProgressingTaskList) {
										this.ProgressingTaskList.Remove(ta);
									}
								});
								ta.OnError.Subscribe(ex => {
									logging.Log("バックグラウンドタスクエラー!", LogLevel.Warning, ex);
									lock (this.ProgressingTaskList) {
										this.ProgressingTaskList.Remove(ta);
									}
								});
								ta.BackgroundStart();

								if (ta is ContinuousTaskAction) {
									return;
								}
								lock (this.ProgressingTaskList) {
									this._taskList[ta.Priority].Remove(ta);
								}
							}
						}
					});
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// タスクの追加
		/// </summary>
		/// <param name="taskAction">追加するタスク</param>
		public void AddTask(ITaskAction taskAction) {
			lock (this._hasTaskLockObj) {
				this._hasTask = true;
				lock (this.ProgressingTaskList) {
					this._taskList[taskAction.Priority].Add(taskAction);
				}
			}
			if (taskAction is ContinuousTaskAction cta) {
				cta.OnRestart.Subscribe(_ => {
					this._hasTask = true;
					lock (this.ProgressingTaskList) {
						this._taskStateChanged.OnNext(Unit.Default);
					}
				});
				cta.OnDisposed.Subscribe(_ => {
					lock (this.ProgressingTaskList) {
						this._taskList[taskAction.Priority].Remove(cta);
					}
				});
			}
		}

		protected override void Dispose(bool disposing) {
			foreach (var ta in this._taskList.SelectMany(x => x.Value).Union(this.ProgressingTaskList)) {
				ta.Dispose();
			}
			base.Dispose(disposing);
		}
	}
}
