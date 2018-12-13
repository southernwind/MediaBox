using System.Windows;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaGroupViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			var fileModel = Get.Instance<MediaFile>("");
			var fileVm = Get.Instance<MediaFileViewModel>(fileModel);
			_ = Get.Instance<MediaGroupViewModel>(fileVm, new Rectangle(new Point(0, 0), new Size(100, 100)));
		}
	}
}
