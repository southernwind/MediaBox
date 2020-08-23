
using NUnit.Framework;

using SandBeige.MediaBox.TestUtilities.MockCreator;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Tests.ViewModels {
	internal class MainWindowStatusBarViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var priorityTaskQueueMock = ModelMockCreator.CreatePriorityTaskQueueMock();
			var notificationManagerMock = ModelMockCreator.CreateNotificationManagerMock();
			using var vm = new MainWindowStatusBarViewModel(priorityTaskQueueMock.Object, notificationManagerMock.Object);
		}
	}
}
