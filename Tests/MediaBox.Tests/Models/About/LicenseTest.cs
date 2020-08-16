using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.About;
namespace SandBeige.MediaBox.Tests.Models.About {
	internal class LicenseTest : ModelTestClassBase {

		[Test]
		public void インスタンス作成() {
			var l = new License("name", "https://sample.com/", "origin");
			l.ProductName.Should().Be("name");
			l.ProjectUrl.Should().Be("https://sample.com/");
			l.LicenseType.Should().Be("origin");
		}
	}
}
