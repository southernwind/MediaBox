﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using SandBeige.MediaBox.Utilities;

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

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		/// <summary>
		/// バックグラウンドタスク処理用タスク
		/// </summary>
		public ReactiveCollection<Task> ProgressList {
			get;
		} = new ReactiveCollection<Task>();

		/// <summary>
		/// 処理状況リスト
		/// </summary>
		public ReadOnlyReactiveCollection<StateObject> ProgressStates {
			get;
		}

		/// <summary>
		/// 処理中カウント
		/// </summary>
		public IReactiveProperty<int> ProgressingCount {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// 完了済みタスク件数
		/// </summary>
		public IReactiveProperty<int> CompletedCount {
			get;
		} = new ReactivePropertySlim<int>();

		/// <summary>
		/// 全タスク件数
		/// </summary>
		public IReactiveProperty<int> TaskCount {
			get;
		} = new ReactivePropertySlim<int>();

		// コンストラクタ
		public PriorityTaskQueue() {
			this.ProgressStates = this.ProgressList.ToReadOnlyReactiveCollection(x => (StateObject)x.AsyncState);
			this.ProgressStates
				.ObserveElementObservableProperty(x => x.Name)
				.Subscribe(x => {
					this.ProgressingCount.Value = this.ProgressStates.Count(s => s.Name.Value != "完了");
				});

			this._taskList
				.ObserveAddChanged<TaskAction>()
				.Subscribe(_ => {
					lock (this.TaskCount) {
						this.TaskCount.Value++;
					}
				});

			this.ProgressingCount
				.Buffer(TimeSpan.FromSeconds(1))
				.Where(x => (x.Count == 0 && this.ProgressStates.Count == 0) || x.All(i => i == 0))
				.Subscribe(_ => {
					lock (this.TaskCount) {
						this.TaskCount.Value = 0;
					}
					lock (this.CompletedCount) {
						this.CompletedCount.Value = 0;
					}
				});
		}

		public void TaskStart() {
			// プロセッサ数分Taskを生成する
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				var task = new Task(stateObj => {
					if (!(stateObj is StateObject state)) {
						return;
					}
					while (true) {
						TaskAction ta;
						lock (this._taskList) {
							ta = this._taskList.OrderBy(x => x.Priority).FirstOrDefault();
							this._taskList.Remove(ta);
						}
						if (ta == null) {
							state.Name.Value = "完了";
							this._taskList.ObserveAddChanged<TaskAction>().Do(x => Console.WriteLine(x)).FirstAsync().Wait();
							continue;
						}
						state.Name.Value = ta.TaskName;
						if (ta.Token.IsCancellationRequested) {
							continue;
						}
						Dispatcher.CurrentDispatcher.Invoke(ta.Do, PriorityToDispatcherPriority(ta.Priority));
						lock (this.CompletedCount) {
							this.CompletedCount.Value++;
						}
					}
				},
				Get.Instance<StateObject>(),
				this._cancellationTokenSource.Token);
				task.Start();
				this.ProgressList.Add(task);
			}

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
