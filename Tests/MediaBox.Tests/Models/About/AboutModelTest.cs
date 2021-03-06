using System.Linq;
using System.Text.RegularExpressions;

using FluentAssertions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.About;

namespace SandBeige.MediaBox.Tests.Models.About {
	internal class AboutModelTest : ModelTestClassBase {
		[Test]
		public void カレントライセンス変更() {
			using var am = new AboutModel();
			am.CurrentLicense.Value.ProductName.Should().Be("MetadataExtractor");
			am.CurrentLicense.Value.ProjectUrl.Should().Be("https://github.com/drewnoakes/metadata-extractor-dotnet");
			am.CurrentLicense.Value.LicenseType.Should().Be("Apache2.0");
			Regex.IsMatch(am.LicenseText.Value, "^\r\n +?Apache License.*", RegexOptions.Singleline).Should().BeTrue();

			am.CurrentLicense.Value = am.Licenses.First(x => x.ProductName == "log4net");

			am.CurrentLicense.Value.ProductName.Should().Be("log4net");
			am.CurrentLicense.Value.ProjectUrl.Should().Be("https://github.com/apache/logging-log4net");
			am.CurrentLicense.Value.LicenseType.Should().Be("Apache2.0");
			Regex.IsMatch(am.LicenseText.Value, @"^ +?Apache License.*apache\.org", RegexOptions.Singleline).Should().BeTrue();
		}
	}
}
