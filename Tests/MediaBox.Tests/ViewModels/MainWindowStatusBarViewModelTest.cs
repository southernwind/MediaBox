using Moq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces.Models.Notification;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class MainWindowStatusBarViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var priorityTaskQueueMock = new Mock<IPriorityTaskQueue>();
			var notificationManagerMock = new Mock<INotificationManager>();
			using var vm = new MainWindowStatusBarViewModel(priorityTaskQueueMock.Object, notificationManagerMock.Object);
		}
	}
}
