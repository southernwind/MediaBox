using NUnit.Framework;

using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaFileInformationsViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<MediaFileInformationViewModel>();
		}
	}
}
