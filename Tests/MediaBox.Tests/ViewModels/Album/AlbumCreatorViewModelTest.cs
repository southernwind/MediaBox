using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;

namespace SandBeige.MediaBox.Tests.ViewModels.Album {
	[TestFixture]
	internal class AlbumCreatorViewModelTest:ViewModelTestClassBase {
		[Test]
		public void Test() {
			_ = Get.Instance<AlbumContainerViewModel>();
		}
	}
}
