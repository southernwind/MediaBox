using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using NUnit.Framework;
using SandBeige.MediaBox.Library.Map;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Media;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaFilePropertiesViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			var vm = Get.Instance<MediaFilePropertiesViewModel>();
		}
	}
}
