
using FluentAssertions;

using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.About;
using SandBeige.MediaBox.TestUtilities.MockCreator;
using SandBeige.MediaBox.ViewModels.About;

namespace SandBeige.MediaBox.Tests.ViewModels.About {
	internal class AboutWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void カレントライセンス変更() {
			var modelMock = ModelMockCreator.CreateAboutModelMock();
			var license1 = ModelMockCreator.CreateLicenseMock();
			license1.Setup(x => x.ProductName).Returns("product1");
			var license2 = ModelMockCreator.CreateLicenseMock();
			license1.Setup(x => x.ProductName).Returns("product2");
			modelMock.Setup(x => x.Licenses).Returns(new ReactiveCollection<ILicense>(){
				license1.Object,
				license2.Object
			});
			modelMock.Setup(x => x.CurrentLicense).Returns(new ReactivePropertySlim<ILicense>(license1.Object));
			modelMock.Setup(x => x.LicenseText).Returns(new ReactivePropertySlim<string>("license mit mit"));
			using var vm = new AboutWindowViewModel(modelMock.Object);
			vm.Licenses.Should().Equal(license1.Object, license2.Object);
			vm.LicenseText.Value.Should().Be("license mit mit");
			vm.CurrentLicense.Value.Should().Be(license1.Object);
		}
	}
}
