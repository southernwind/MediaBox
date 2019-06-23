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
		public async Task タスク追加() {
			this.TaskQueue.TaskCount.Value.Is(0);
			var cts = new CancellationTokenSource();
			var count = 0;
			var ta = new TaskAction("name1", async state => await Task.Run(() => {
				Thread.Sleep(100);
				count++;
			}), Priority.LoadFullImage, cts.Token);
			this.TaskQueue.AddTask(ta);
			await RxUtility.WaitPolling(() => this.TaskQueue.TaskCount.Value != 0, 10, 100);
			this.TaskQueue.TaskCount.Value.Is(1);

			await ta.OnTaskCompleted.FirstAsync();
			count.Is(1);

			this.TaskQueue.TaskCount.Value.Is(0);
		}


		[Test]
		public async Task タスク処理順() {
			this.TaskQueue.TaskCount.Value.Is(0);
			var cts = new CancellationTokenSource();
			var result = new List<int>();
			// タスクなし状態でのタスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				var ta = new TaskAction("name1", async state => await Task.Run(() => {
					Thread.Sleep(10);
					lock (result) {
						result.Add(1);
					}
				}), Priority.LoadMediaFiles, cts.Token);
				this.TaskQueue.AddTask(ta);
			}

			//タスクあり状態での低優先度タスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				this.TaskQueue.AddTask(new TaskAction("name2", async state => await Task.Run(() => {
					Thread.Sleep(300);
					lock (result) {
						result.Add(2);
					}
				}), Priority.LoadMediaFiles, cts.Token));
			}

			//タスクあり状態での高優先度タスク追加
			foreach (var _ in Enumerable.Range(0, Environment.ProcessorCount)) {
				this.TaskQueue.AddTask(new TaskAction("name3", async state => await Task.Run(() => {
					Thread.Sleep(10);
					lock (result) {
						result.Add(3);
					}
				}), Priority.LoadFullImage, cts.Token));
			}

			await RxUtility.WaitPolling(() => result.Count() == Environment.ProcessorCount * 3, 10, 2000);
			result.Skip(Environment.ProcessorCount * 2).Take(Environment.ProcessorCount).Is(Enumerable.Repeat(2, Environment.ProcessorCount));
		}

		[Test]
		public async Task タスク処理完了通知() {
			this.TaskQueue.TaskCount.Value.Is(0);
			var cts = new CancellationTokenSource();

			var are1 = new AutoResetEvent(false);
			var ta1 = new TaskAction("name", async state => await Task.Run(() => {
				are1.WaitOne();
			}), Priority.LoadFullImage, cts.Token);
			this.TaskQueue.AddTask(ta1);

			var are2 = new AutoResetEvent(false);
			var ta2 = new TaskAction("name2", async state => await Task.Run(() => {
				are2.WaitOne();
			}), Priority.LoadFullImage, cts.Token);
			this.TaskQueue.AddTask(ta2);

			Assert.ThrowsAsync<TimeoutException>(async () => {
				await this.WaitTaskCompleted(3000);
			});
			are1.Set();
			Assert.ThrowsAsync<TimeoutException>(async () => {
				await this.WaitTaskCompleted(3000);
			});
			are2.Set();
			await this.WaitTaskCompleted(100);
		}

		[Test]
		public async Task 継続タスク() {
			var cts = new CancellationTokenSource();

			var are = new AutoResetEvent(false);
			var cta = new ContinuousTaskAction("name", async state => await Task.Run(() => {
				Console.WriteLine("11");
				are.WaitOne();
			}), Priority.LoadFullImage, cts.Token);
			this.TaskQueue.AddTask(cta);

			Assert.ThrowsAsync<TimeoutException>(async () => {
				await this.WaitTaskCompleted(3000);
			});
			this.TaskQueue.TaskCount.Value.Is(1);
			are.Set();
			await this.WaitTaskCompleted(100);
			this.TaskQueue.TaskCount.Value.Is(0);
			are.Reset();
			Console.WriteLine("restart");
			cta.Restart();
			this.TaskQueue.TaskCount.Value.Is(1);
			are.Set();
			await this.WaitTaskCompleted(100);
			this.TaskQueue.TaskCount.Value.Is(0);

		}
	}
}
