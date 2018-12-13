using NUnit.Framework;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class NavigationMenuViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<NavigationMenuViewModel>();
		}
	}
}
