using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Composition.Logging;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// 優先度付きタスク処理機構
	/// </summary>
	/// <remarks>
	/// DIコンテナによってシングルトンとして管理され、優先度の高いものから順に処理をしていく。
	/// </remarks>
	internal class PriorityTaskQueue : ModelBase, IEnumerable<TaskAction>, IDisposable {
		/// <summary>
		/// バックグラウンドタスクリスト
		/// </summary>
		private readonly ObservableSynchronizedCollection<TaskAction> _taskList = new ObservableSynchronizedCollection<TaskAction>();

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

		public ReactiveCollection<TaskAction> ProgressingTaskList {
			get;
		} = new ReactiveCollection<TaskAction>();

		/// <summary>
		/// タスク件数
		/// </summary>
		public IReactiveProperty<int> TaskCount {
			get;
		} = new ReactivePropertySlim<int>();

		// コンストラクタ
		public PriorityTaskQueue() {
			this._taskList
				.CollectionChangedAsObservable()
				.Subscribe(_ => {
					this.TaskCount.Value = this._taskList.Count;
				}).AddTo(this.CompositeDisposable);

			// 新たにタスクが追加されたり、実行中タスクが完了したタイミングで新しいタスクを実行するかを検討する。
			this.ProgressingTaskList.CollectionChangedAsObservable()
				.Merge(this._taskList.CollectionChangedAsObservable())
				.ObserveOn(TaskPoolScheduler.Default)
				.Synchronize()
				.Subscribe(_ => {
					if (this._taskList.Count == 0 && this.ProgressingTaskList.Count == 0) {
						this._allTaskCompletedSubject.OnNext(Unit.Default);
					}
					if (this.ProgressingTaskList.Count > 5) {
						return;
					}
					TaskAction ta;
					lock (this._taskList) {
						ta =
							this
								._taskList
								.Where(x => x.TaskStartCondition() && x.TaskState == TaskState.Waiting)
								.OrderBy(x => x.Priority)
								.FirstOrDefault();
						ta?.Reserve();
					}

					if (ta != null) {
						lock (this.ProgressingTaskList) {
							this.ProgressingTaskList.Add(ta);
						}
						ta.OnTaskCompleted.Subscribe(_ => {
							lock (this.ProgressingTaskList) {
								this.ProgressingTaskList.Remove(ta);
							}
						});
						ta.OnErrorSubject.Subscribe(ex => {
							this.Logging.Log("バックグラウンドタスクエラー!", LogLevel.Warning, ex);
							lock (this.ProgressingTaskList) {
								this.ProgressingTaskList.Remove(ta);
							}
						});
						ta.BackgroundStart();
						this._taskList.Remove(ta);
					}
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// タスクの追加
		/// </summary>
		/// <param name="taskAction">追加するタスク</param>
		public void AddTask(TaskAction taskAction) {
			this._taskList.Add(taskAction);
		}

		/// <summary>
		/// イテレーター取得
		/// </summary>
		/// <returns>イテレーター</returns>
		public IEnumerator<TaskAction> GetEnumerator() {
			lock (this._taskList) {
				return this._taskList.ToList().GetEnumerator();
			}
		}

		/// <summary>
		/// イテレーター取得
		/// </summary>
		/// <returns>イテレーター</returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}
}
