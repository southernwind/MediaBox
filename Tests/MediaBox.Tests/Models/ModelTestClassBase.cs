using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.TestUtilities;
namespace SandBeige.MediaBox.Tests.Models {
	internal class ModelTestClassBase : TestClassBase {

		[OneTimeSetUp]
		public override void OneTimeSetUp() {
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
			base.OneTimeSetUp();
		}

		[SetUp]
		public override void SetUp() {
			ReactivePropertyScheduler.SetDefault(ImmediateScheduler.Instance);
			base.SetUp();
		}

		[TearDown]
		public override void TearDown() {
			base.TearDown();
		}

		/// <summary>
		/// タスク完了待機
		/// </summary>
		/// <param name="taskQueue">待機するタスクキュー</param>
		/// <param name="timeoutMilliSeconds">タイムアウトまでの秒数</param>
		/// <returns>タスク</returns>
		protected async Task WaitTaskCompleted(IPriorityTaskQueue taskQueue, int timeoutMilliSeconds) {
			Console.WriteLine($"タスク残件数:{taskQueue.TaskCount.Value}");
			RxUtility.WaitScheduler(TaskPoolScheduler.Default);
			if (taskQueue.TaskCount.Value == 0) {
				return;
			}
			await
				taskQueue
					.AllTaskCompleted
					.FirstAsync()
					.Timeout(TimeSpan.FromMilliseconds(timeoutMilliSeconds));
		}
	}
}
