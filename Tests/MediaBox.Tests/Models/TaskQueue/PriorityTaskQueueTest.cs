using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.TaskQueue {
	internal class PriorityTaskQueueTest : ModelTestClassBase {
		public override void SetUp() {
			base.SetUp();
			this.UseDataBaseFile();
			RxUtility.WaitScheduler(ReactivePropertyScheduler.Default);
		}

		[Test]
		public void タスク準備() {
			this.TaskQueue.ProgressList.Count.Is(Environment.ProcessorCount);
			this.TaskQueue.ProgressStates.Count.Is(Environment.ProcessorCount);

			this.TaskQueue.ProgressStates.All(x => x.Name.Value == "完了").IsTrue();
		}

		[Test]
		public async Task タスク追加() {
			this.TaskQueue.Count().Is(0);
			var cts = new CancellationTokenSource();
			var count = 0;
			var ta = new TaskAction("name1", () => {
				Thread.Sleep(10);
				count++;
			}, Priority.LoadFullImage, cts.Token);
			this.TaskQueue.AddTask(ta);
			this.TaskQueue.TaskCount.Value.Is(1);

			await ta.OnTaskCompleted.FirstAsync();
			count.Is(1);
			await RxUtility.WaitPolling(() => this.TaskQueue.TaskCount.Value == 0, 10, 100);

			this.TaskQueue.TaskCount.Value.Is(0);
		}


		[Test]
		public async Task タスク処理順() {
			this.TaskQueue.Count().Is(0);
			var cts = new CancellationTokenSource();
			var count1 = 0;
			var count2 = 0;
			var count3 = 0;
			var lockObj = new object();
			// タスクなし状態でのタスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				var ta = new TaskAction("name1", () => {
					Thread.Sleep(10);
					lock (lockObj) {
						count1++;
					}
				}, Priority.LoadRegisteredAlbumOnRegister, cts.Token);
				this.TaskQueue.AddTask(ta);
			}

			//タスクあり状態での低優先度タスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				this.TaskQueue.AddTask(new TaskAction("name2", () => {
					Thread.Sleep(500);
					lock (lockObj) {
						count2++;
					}
				}, Priority.LoadRegisteredAlbumOnRegister, cts.Token));
			}

			//タスクあり状態での高優先度タスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				this.TaskQueue.AddTask(new TaskAction("name3", () => {
					Thread.Sleep(10);
					lock (lockObj) {
						count3++;
					}
				}, Priority.LoadFullImage, cts.Token));
			}

			await RxUtility.WaitPolling(() => count3 == Environment.ProcessorCount, 10, 300);
			count2.Is(0);

			await RxUtility.WaitPolling(() => count2 == Environment.ProcessorCount, 10, 1000);
		}
	}
}
