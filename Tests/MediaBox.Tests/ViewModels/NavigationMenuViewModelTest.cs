using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
