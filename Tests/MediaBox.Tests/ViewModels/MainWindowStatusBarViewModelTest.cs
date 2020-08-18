using Moq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class MainWindowStatusBarViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var loggingMock = new Mock<ILogging>();
			var priorityTaskQueueMock = new Mock<PriorityTaskQueue>(() => new PriorityTaskQueue(loggingMock.Object));
			var notificationManagerMock = new Mock<NotificationManager>(() => new NotificationManager());
			using var vm = new MainWindowStatusBarViewModel(priorityTaskQueueMock.Object, notificationManagerMock.Object);
		}
	}
}
