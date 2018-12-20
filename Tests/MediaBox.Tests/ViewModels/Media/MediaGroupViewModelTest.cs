using System.Windows;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaGroupViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			var media = Get.Instance<MediaFile>("");
			_ = Get.Instance<MediaGroupViewModel>(Get.Instance<MediaGroup>(media, default(Rectangle)));
		}
	}
}
