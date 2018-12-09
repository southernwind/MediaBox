using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Media;
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
