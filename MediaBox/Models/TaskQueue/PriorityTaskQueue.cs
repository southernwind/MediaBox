using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;

using Livet;

using Reactive.Bindings.Extensions;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// 優先度付きタスク処理機構
	/// </summary>
	/// <remarks>
	/// DIコンテナによってシングルトンとして管理され、優先度の高いものから順に処理をしていく。
	/// </remarks>
	internal class PriorityTaskQueue : ModelBase, IEnumerable<TaskAction> {
		/// <summary>
		/// バックグラウンドタスクリスト
		/// </summary>
		private readonly ObservableSynchronizedCollection<TaskAction> _taskList = new ObservableSynchronizedCollection<TaskAction>();

		private int _count = 0;

		// コンストラクタ
		public PriorityTaskQueue() {
			var lockObj = new object();
			// Task消化
			this._taskList
				.ObserveAddChanged<TaskAction>()
				.ObserveOn(ThreadPoolScheduler.Instance)
				.Where(_ => this._count < Environment.ProcessorCount)
				.Subscribe(_ => {
					lock (lockObj) {
						this._count++;
					}
					TaskAction ta;
					lock (this._taskList) {
						ta = this._taskList.OrderBy(x => x.Priority).FirstOrDefault();
						this._taskList.Remove(ta);
					}
					if (ta == null) {
						return;
					}
					if (ta.Token.IsCancellationRequested) {
						return;
					}
					Dispatcher.CurrentDispatcher.Invoke(ta.Do, DispatcherPriority.Background);
					lock (lockObj) {
						this._count--;
					}
				});
		}

		/// <summary>
		/// タスクの追加
		/// </summary>
		/// <param name="taskAction">追加するタスク</param>
		public void AddTask(TaskAction taskAction) {
			lock (this._taskList) {
				this._taskList.Add(taskAction);
			}
		}

		/// <summary>
		/// タスクの削除
		/// </summary>
		/// <param name="taskAction">削除するタスク</param>
		public void RemoveTask(TaskAction taskAction) {
			lock (this._taskList) {
				this._taskList.Remove(taskAction);
			}
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
