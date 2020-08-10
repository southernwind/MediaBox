using System;
using System.Reactive;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue {
	public interface IPriorityTaskQueue {
		IObservable<Unit> AllTaskCompleted {
			get;
		}
		ReactiveCollection<ITaskAction> ProgressingTaskList {
			get;
		}
		IReactiveProperty<int> TaskCount {
			get;
		}

		void AddTask(ITaskAction taskAction);
	}
}