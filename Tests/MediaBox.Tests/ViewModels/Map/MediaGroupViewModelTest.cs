using NUnit.Framework;

using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Map;

namespace SandBeige.MediaBox.Tests.ViewModels.Map {
	[TestFixture]
	internal class MediaGroupViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Core() {
			var core = Get.Instance<MediaFile>("");
			var model = Get.Instance<MediaGroup>(core, default(Rectangle));
			var vm = Get.Instance<MediaGroupViewModel>(model);
			vm.Core.Value.Model.Is(core);
		}
	}
}
