using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue.Objects;

namespace SandBeige.MediaBox.Models.TaskQueue {
	/// <summary>
	/// 継続実行可能なタスク定義
	/// </summary>
	public class ContinuousTaskAction : TaskAction {
		private readonly Subject<Unit> _onRestart = new Subject<Unit>();

		public IObservable<Unit> OnRestart {
			get {
				return this._onRestart.AsObservable();
			}
		}

		public ContinuousTaskAction(string taskName, Func<TaskActionState, Task> action, Priority priority, CancellationTokenSource cancellationTokenSource, Func<bool> taskStartCondition = null)
			: base(taskName, action, priority, cancellationTokenSource, taskStartCondition) {
		}

		public void Restart() {
			if (this.TaskState == TaskState.Done) {
				this.TaskState = TaskState.Waiting;
			}
			this._onRestart.OnNext(Unit.Default);
		}
	}
}
