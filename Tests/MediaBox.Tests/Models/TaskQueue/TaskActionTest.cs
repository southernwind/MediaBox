using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using SandBeige.MediaBox.Models.TaskQueue;

namespace SandBeige.MediaBox.Tests.Models.TaskQueue {
	internal class TaskActionTest : ModelTestClassBase {
		[Test]
		public void インスタンス作成() {
			var cts = new CancellationTokenSource();
			var ta = new TaskAction("Name", () => {

			}, Priority.LoadRegisteredAlbumOnLoad, cts.Token);

			ta.TaskName.Is("Name");
			ta.Priority.Is(Priority.LoadRegisteredAlbumOnLoad);
		}

		[Test]
		public async Task ステータス() {
			var cts = new CancellationTokenSource();
			var ta = new TaskAction("Name", () => {
				Thread.Sleep(1000);
			}, Priority.LoadRegisteredAlbumOnLoad, cts.Token);
			ta.TaskState.Is(TaskState.Waiting);
			ta.Reserve();
			ta.TaskState.Is(TaskState.Reserved);

			var task = Observable.Start(ta.Do, NewThreadScheduler.Default);
			Thread.Sleep(500);
			ta.TaskState.Is(TaskState.WorkInProgress);

			await task;
			ta.TaskState.Is(TaskState.Done);
		}

		[Test]
		public void 実行() {
			var cts = new CancellationTokenSource();
			var flag = false;
			var ta = new TaskAction("Name", () => {
				flag = true;
			}, Priority.LoadRegisteredAlbumOnLoad, cts.Token, () => flag);
			ta.Reserve();
			flag.IsFalse();
			ta.Do();
			flag.IsTrue();
		}
	}
}
