﻿using System;
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
		private readonly Dictionary<Priority, ObservableSynchronizedCollection<TaskAction>> _taskList = new Dictionary<Priority, ObservableSynchronizedCollection<TaskAction>>();

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
			var taskListChanged = new Subject<Unit>();
			foreach (var p in Enum.GetValues(typeof(Priority)).OfType<Priority>().OrderBy(x => x)) {
				var osc = new ObservableSynchronizedCollection<TaskAction>();
				this._taskList.Add(p, osc);
				osc.CollectionChangedAsObservable().Subscribe(_ => taskListChanged.OnNext(Unit.Default));
			}

			// 新たにタスクが追加されたり、実行中タスクが完了したタイミングで新しいタスクを実行するかを検討する。
			this.ProgressingTaskList.CollectionChangedAsObservable().ToUnit()
				.Merge(taskListChanged)
				.Merge(this._taskStateChanged)
				.ObserveOn(TaskPoolScheduler.Default)
				.Subscribe(_ => {
					// タスク件数の更新
					lock (this.ProgressingTaskList) {
						this.TaskCount.Value =
							this._taskList
								.SelectMany(x => x.Value)
								.Union(this.ProgressingTaskList)
								.Where(x => x.TaskState != TaskState.Done)
								.Count();
					}
					if (this.TaskCount.Value == 0) {
						this._allTaskCompletedSubject.OnNext(Unit.Default);
						return;
					}

					if (this.ProgressingTaskList.Count > 5) {
						return;
					}
					TaskAction ta;
					lock (this._taskList) {
						ta =
							this
								._taskList
								.SelectMany(x => x.Value)
								.Where(x => x.TaskStartCondition() && x.TaskState == TaskState.Waiting)
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
						ta.OnError.Subscribe(ex => {
							this.Logging.Log("バックグラウンドタスクエラー!", LogLevel.Warning, ex);
							lock (this.ProgressingTaskList) {
								this.ProgressingTaskList.Remove(ta);
							}
						});
						ta.BackgroundStart();

						if (ta is ContinuousTaskAction) {
							return;
						}
						this._taskList[ta.Priority].Remove(ta);
					}
				}).AddTo(this.CompositeDisposable);
		}

		/// <summary>
		/// タスクの追加
		/// </summary>
		/// <param name="taskAction">追加するタスク</param>
		public void AddTask(TaskAction taskAction) {
			this._taskList[taskAction.Priority].Add(taskAction);
			if (taskAction is ContinuousTaskAction cta) {
				cta.OnRestart.Subscribe(this._taskStateChanged.OnNext);
				cta.OnDisposed.Subscribe(_ => {
					this._taskList[taskAction.Priority].Remove(cta);
				});
			}
		}

		/// <summary>
		/// イテレーター取得
		/// </summary>
		/// <returns>イテレーター</returns>
		public IEnumerator<TaskAction> GetEnumerator() {
			TaskAction[] ptl;
			lock (this.ProgressingTaskList) {
				ptl = this.ProgressingTaskList.ToArray();
			}
			lock (this._taskList) {
				return this._taskList.SelectMany(x => x.Value).Union(ptl).ToList().GetEnumerator();
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
