using System.Linq;
using System.Text.RegularExpressions;

using NUnit.Framework;

using SandBeige.MediaBox.Models.About;
using SandBeige.MediaBox.ViewModels.About;

namespace SandBeige.MediaBox.Tests.ViewModels.About {
	internal class AboutWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void カレントライセンス変更() {
			using var model = new AboutModel();
			using var vm = new AboutWindowViewModel(model);
			vm.Licenses.Is(model.Licenses);
			vm.LicenseText.Value.Is(model.LicenseText.Value);
			vm.CurrentLicense.Value.Is(model.CurrentLicense.Value);

			vm.CurrentLicense.Value = model.Licenses.First(x => x.ProductName == "log4net");
			vm.LicenseText.Value.Is(model.LicenseText.Value);
			Regex.IsMatch(vm.LicenseText.Value, @"^ +?Apache License.*apache\.org", RegexOptions.Singleline).IsTrue();
		}

	}
}
