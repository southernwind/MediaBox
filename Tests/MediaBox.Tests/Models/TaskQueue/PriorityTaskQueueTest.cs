using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.TestUtilities;

namespace SandBeige.MediaBox.Tests.Models.TaskQueue {
	internal class PriorityTaskQueueTest : ModelTestClassBase {

		[Test]
		public async Task タスク準備() {
			this.TaskQueue.ProgressList.Count.Is(Environment.ProcessorCount);
			await RxUtility.WaitPolling(() => this.TaskQueue.ProgressStates.Count >= Environment.ProcessorCount, 10, 300);
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
			var result = new List<int>();
			// タスクなし状態でのタスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				var ta = new TaskAction("name1", () => {
					Thread.Sleep(10);
					lock (result) {
						result.Add(1);
					}
				}, Priority.LoadMediaFiles, cts.Token);
				this.TaskQueue.AddTask(ta);
			}

			//タスクあり状態での低優先度タスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				this.TaskQueue.AddTask(new TaskAction("name2", () => {
					Thread.Sleep(300);
					lock (result) {
						result.Add(2);
					}
				}, Priority.LoadMediaFiles, cts.Token));
			}

			//タスクあり状態での高優先度タスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				this.TaskQueue.AddTask(new TaskAction("name3", () => {
					Thread.Sleep(10);
					lock (result) {
						result.Add(3);
					}
				}, Priority.LoadFullImage, cts.Token));
			}

			await RxUtility.WaitPolling(() => result.Count() == Environment.ProcessorCount * 3, 10, 2000);
			result.Skip(Environment.ProcessorCount * 2).Take(Environment.ProcessorCount).Is(Enumerable.Repeat(2, Environment.ProcessorCount));
		}
	}
}
