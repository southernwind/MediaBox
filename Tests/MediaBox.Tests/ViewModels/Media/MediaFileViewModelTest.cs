using System.IO;

using NUnit.Framework;

namespace SandBeige.MediaBox.Tests.ViewModels.Media {
	[TestFixture]
	internal class MediaFileViewModelTest : ViewModelTestClassBase {
		[Test]
		public void Test() {
			var model = this.MediaFactory.Create(Path.Combine(TestDataDir, "image1.jpg"));
			_ = this.ViewModelFactory.Create(model);
		}
	}
}
