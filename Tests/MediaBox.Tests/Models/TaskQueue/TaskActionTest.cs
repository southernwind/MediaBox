using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Tests.Models.TaskQueue {
	internal class TaskActionTest : ModelTestClassBase {
		[Test]
		public void インスタンス作成() {
			using var cts = new CancellationTokenSource();
			using var ta = new TaskAction("Name", async state => await Task.Run(() => { }), Priority.LoadMediaFiles, cts);

			ta.TaskName.Value.Is("Name");
			ta.Priority.Is(Priority.LoadMediaFiles);
		}

		[Test]
		public async Task ステータス() {
			using var cts = new CancellationTokenSource();
			using var ta = new TaskAction("Name", async state => await Task.Delay(1000), Priority.LoadMediaFiles, cts);
			ta.TaskState.Is(TaskState.Waiting);
			ta.Reserve();
			ta.TaskState.Is(TaskState.Reserved);

			var task = ta.DoAsync();
			Thread.Sleep(500);
			ta.TaskState.Is(TaskState.WorkInProgress);

			await task;
			ta.TaskState.Is(TaskState.Done);
		}

		[Test]
		public async Task 実行() {
			using var cts = new CancellationTokenSource();
			var flag = false;
			using var ta = new TaskAction("Name", async state => await Task.Run(() => {
				flag = true;
			}), Priority.LoadMediaFiles, cts, () => flag);
			ta.Reserve();
			flag.IsFalse();
			await ta.DoAsync();
			flag.IsTrue();
		}
	}
}
