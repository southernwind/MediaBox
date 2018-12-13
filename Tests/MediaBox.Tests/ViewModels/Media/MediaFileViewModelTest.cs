using NUnit.Framework;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaFileViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			var model = Get.Instance<MediaFile>("");
			_ = Get.Instance<MediaFileViewModel>(model);
		}
	}
}
