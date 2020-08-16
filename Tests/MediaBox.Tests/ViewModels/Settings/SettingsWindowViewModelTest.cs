using Moq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.Settings;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings {
	[TestFixture]
	internal class SettingsWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var settingsMock = new Mock<ISettings>();
			var viewModelFactoryMock = new Mock<ViewModelFactory>();
			var pluginManagerMock = new Mock<IPluginManager>();
			using var _ = new SettingsWindowViewModel(settingsMock.Object, viewModelFactoryMock.Object, pluginManagerMock.Object);
		}
	}
}
