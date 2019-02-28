using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
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

		// コンストラクタ
		public PriorityTaskQueue() {

			var waitHandle = new AutoResetEvent(false);

			// プロセッサ数分Taskを生成する
			foreach (var __ in Enumerable.Range(0, Environment.ProcessorCount)) {
				Task.Run(() => {
					while (true) {
						TaskAction ta;
						lock (this._taskList) {
							ta = this._taskList.OrderBy(x => x.Priority).FirstOrDefault();
							this._taskList.Remove(ta);
						}
						if (ta == null) {
							waitHandle.WaitOne();
							continue;
						}
						if (ta.Token.IsCancellationRequested) {
							continue;
						}
						Dispatcher.CurrentDispatcher.Invoke(ta.Do, PriorityToDispatcherPriority(ta.Priority));
					}
				});
			}

			this._taskList
				.ObserveAddChanged<TaskAction>()
				.Subscribe(_ => waitHandle.Set());
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

		/// <summary>
		/// 優先度変換<see cref="Priority"/>-><see cref="DispatcherPriority"/>
		/// </summary>
		/// <param name="priority">変換前優先度</param>
		/// <returns>変換後優先度</returns>
		private static DispatcherPriority PriorityToDispatcherPriority(Priority priority) {
			switch (priority) {
				case Priority.LoadFullImage:
					return DispatcherPriority.Background;
				case Priority.LoadFolderAlbumFileInfo:
				case Priority.LoadRegisteredAlbumOnLoad:
				case Priority.LoadRegisteredAlbumOnRegister:
					return DispatcherPriority.ContextIdle;
			}
			return DispatcherPriority.ApplicationIdle;
		}
	}
}
