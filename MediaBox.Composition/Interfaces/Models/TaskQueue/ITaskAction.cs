using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue {
	public interface ITaskAction : IModelBase, IComparable {
		CancellationTokenSource CancellationTokenSource {
			get;
		}
		IReadOnlyReactiveProperty<bool> IsIndeterminate {
			get;
		}
		IObservable<Exception> OnError {
			get;
		}
		IObservable<Unit> OnTaskCompleted {
			get;
		}
		Priority Priority {
			get;
		}
		IReactiveProperty<double?> ProgressMax {
			get;
		}
		IReadOnlyReactiveProperty<double> ProgressRate {
			get;
		}
		IReactiveProperty<double> ProgressValue {
			get;
		}
		IReactiveProperty<string> TaskName {
			get;
		}
		Func<bool> TaskStartCondition {
			get;
		}
		TaskState TaskState {
			get;
			set;
		}

		void BackgroundStart();
		Task DoAsync();
		void Reserve();
		string ToString();
		Task Wait();
	}
}