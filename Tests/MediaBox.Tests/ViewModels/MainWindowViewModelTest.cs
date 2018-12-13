using NUnit.Framework;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class MainWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<MainWindowViewModel>();
		}
	}
}
