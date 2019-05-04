using NUnit.Framework;

using SandBeige.MediaBox.Models.About;

namespace SandBeige.MediaBox.Tests.Models.About {
	internal class LicenseTest : ModelTestClassBase {

		[Test]
		public void インスタンス作成() {
			var l = new License("name", "https://sample.com/", "origin");
			l.ProductName.Is("name");
			l.ProjectUrl.Is("https://sample.com/");
			l.LicenseType.Is("origin");
		}
	}
}
