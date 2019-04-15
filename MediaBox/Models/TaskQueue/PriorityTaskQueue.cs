using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

using Livet;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Helpers;

using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// 優先度付きタスク処理機構
	/// </summary>
	/// <remarks>
	/// DIコンテナによってシングルトンとして管理され、優先度の高いものから順に処理をしていく。
	/// </remarks>
	internal class PriorityTaskQueue : IEnumerable<TaskAction>, IDisposable {
		/// <summary>
		/// バックグラウンドタスクリスト
		/// </summary>
		private readonly ObservableSynchronizedCollection<TaskAction> _taskList = new ObservableSynchronizedCollection<TaskAction>();

		private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		/// <summary>
		/// Dispose通知用Subject
		/// </summary>
		private readonly Subject<Unit> _onDisposed = new Subject<Unit>();

		/// <summary>
		/// Dispose済みか否か
		/// </summary>
		private bool _disposed;

		private readonly CompositeDisposable CompositeDisposable = new CompositeDisposable();

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
		/// タスク件数
		/// </summary>
		public IReactiveProperty<int> TaskCount {
			get;
		} = new ReactivePropertySlim<int>();

		// コンストラクタ
		public PriorityTaskQueue() {
			this.ProgressStates = this.ProgressList.ToReadOnlyReactiveCollection(x => (StateObject)x.AsyncState).AddTo(this.CompositeDisposable);

			this._taskList
				.CollectionChangedAsObservable()
				.Subscribe(_ => {
					this.TaskCount.Value = this._taskList.Count;
				}).AddTo(this.CompositeDisposable);
		}

		public void TaskStart() {
			// プロセッサ数分Taskを生成する
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				var task = new Task(stateObj => {
					if (!(stateObj is StateObject state)) {
						return;
					}
					while (true) {
						if (this._disposed) {
							return;
						}
						TaskAction ta;
						lock (this._taskList) {
							ta = this._taskList.Where(x => x.TaskStartCondition()).OrderBy(x => x.Priority).FirstOrDefault();
							this._taskList.Remove(ta);
						}
						if (ta == null) {
							state.Name.Value = "完了";
							this._taskList
								.ObserveAddChanged<TaskAction>()
								.ToUnit()
								.Merge(
									this._taskList.Any()
										? Observable
											.Timer(TimeSpan.FromMilliseconds(100))
											.Where(x => this._taskList.Any(t => t.TaskStartCondition()))
											.ToUnit()
										: Observable.Never<Unit>()
								)
								.Merge(this._onDisposed)
								.FirstAsync()
								.Wait();
							continue;
						}
						state.Name.Value = ta.TaskName;
						if (ta.Token.IsCancellationRequested) {
							continue;
						}
						Dispatcher.CurrentDispatcher.Invoke(ta.Do, PriorityToDispatcherPriority(ta.Priority));
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
		/// Dispose
		/// </summary>
		public void Dispose() {
			this._disposed = true;
			this.CompositeDisposable.Dispose();
			this._onDisposed.OnNext(Unit.Default);
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
