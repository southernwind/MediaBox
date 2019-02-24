using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

using Livet;

using SandBeige.MediaBox.Library.Extensions;

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

		/// <summary>
		/// タスク処理開始用サブジェクト
		/// </summary>
		private readonly Subject<Unit> _beginTask = new Subject<Unit>();

		// コンストラクタ
		public PriorityTaskQueue() {
			// Task消化
			this._beginTask
				.ObserveOnBackground(this.Settings.ForTestSettings.RunOnBackground.Value)
				.Synchronize()
				.Where(_=>this._taskList.Count != 0)
				.Subscribe(_ => {
#if BACKGROUND_LOG
					this.Logging.Log("Start Task");
#endif
					var completed = false;
					var taskList = new List<Task>();
					for (var i = 0; i < Environment.ProcessorCount / 2; i++) {
						taskList.Add(Task.Run(() => {
							while (true) {
								TaskAction ta;
								lock (this._taskList) {
									ta = this._taskList.OrderBy(x => x.Priority).FirstOrDefault();
									this._taskList.Remove(ta);
#if BACKGROUND_LOG
									this.Logging.Log(ta?.Priority);
#endif
								}
								if (ta == null) {
									break;
								}
								if (ta.Token.IsCancellationRequested) {
									continue;
								}
								ta.Do();
								if (completed) {
									break;
								}
							}
							completed = true;
						}));
					}
					Task.WhenAll(taskList).Wait();
#if BACKGROUND_LOG
					this.Logging.Log("End Task");
#endif
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
		/// 追加したタスクの開始
		/// </summary>
		public void StartTask() {
			this._beginTask.OnNext(Unit.Default);
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
