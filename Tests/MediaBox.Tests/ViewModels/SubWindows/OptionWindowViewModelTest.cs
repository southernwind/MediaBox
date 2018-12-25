using NUnit.Framework;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.SubWindows.OptionWindow;

namespace SandBeige.MediaBox.Tests.ViewModels.SubWindows {
	[TestFixture]
	internal class OptionWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<OptionWindowViewModel>();
		}
	}
}
