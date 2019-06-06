using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// 継続実行可能なタスク定義
	/// </summary>
	internal class ContinuousTaskAction : TaskAction {
		private readonly Subject<Unit> _onRestart = new Subject<Unit>();
		private bool _haveToRun = false;

		public IObservable<Unit> OnRestart {
			get {
				return this._onRestart.AsObservable();
			}
		}

		public ContinuousTaskAction(string taskName, Func<Task> action, Priority priority, CancellationToken token, Func<bool> taskStartCondition = null)
			: base(taskName, action, priority, token, taskStartCondition) {

			this.OnTaskCompletedSubject.Where(_ => this._haveToRun).Subscribe(_ => {
				this.TaskState = TaskState.Waiting;
				this._haveToRun = false;
			});
		}

		public void Restart() {
			this._haveToRun = true;
			if (this.TaskState == TaskState.Done) {
				this.TaskState = TaskState.Waiting;
			}
			this._onRestart.OnNext(Unit.Default);
		}
	}
}
