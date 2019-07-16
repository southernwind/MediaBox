using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Tests.Models.TaskQueue {
	internal class PriorityTaskQueueTest : ModelTestClassBase {

		[Test]
		public async Task タスク追加() {
			this.TaskQueue.TaskCount.Value.Is(0);
			using var cts = new CancellationTokenSource();
			var count = 0;
			using var ta = new TaskAction("name1", async state => await Task.Run(() => {
				Thread.Sleep(100);
				count++;
			}), Priority.LoadFullImage, cts);
			this.TaskQueue.AddTask(ta);
			this.TaskQueue.TaskCount.Value.Is(1);

			await ta.OnTaskCompleted.FirstAsync();
			count.Is(1);

			this.TaskQueue.TaskCount.Value.Is(0);
		}

		[Test]
		public async Task タスク処理完了通知() {
			this.TaskQueue.TaskCount.Value.Is(0);
			using var cts = new CancellationTokenSource();

			using var are1 = new AutoResetEvent(false);
			using var ta1 = new TaskAction("name", async state => await Task.Run(() => {
				are1.WaitOne();
			}), Priority.LoadFullImage, cts);
			this.TaskQueue.AddTask(ta1);

			using var are2 = new AutoResetEvent(false);
			using var ta2 = new TaskAction("name2", async state => await Task.Run(() => {
				are2.WaitOne();
			}), Priority.LoadFullImage, cts);
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

			using var are = new AutoResetEvent(false);
			using var cta = new ContinuousTaskAction("name", async state => await Task.Run(() => {
				Console.WriteLine("11");
				are.WaitOne();
			}), Priority.LoadFullImage, cts);
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
