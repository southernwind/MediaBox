
using NUnit.Framework;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.TestUtilities.MockCreator;
using SandBeige.MediaBox.ViewModels.Settings;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings {
	[TestFixture]
	internal class SettingsWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var settingsMock = ModelMockCreator.CreateSettingsMock();

			var viewModelFactoryMock = ModelMockCreator.CreateViewModelFactoryMock();
			var pluginManagerMock = ModelMockCreator.CreatePluginManagerMock();
			var folderSelectionServiceMock = ModelMockCreator.CreateFolderSelectionDialogServiceMock();
			pluginManagerMock.Setup(x => x.PluginList).Returns(new ReactiveCollection<IPluginModel>().ToReadOnlyReactiveCollection());
			using var _ = new SettingsWindowViewModel(settingsMock.Object, viewModelFactoryMock.Object, pluginManagerMock.Object, folderSelectionServiceMock.Object);
		}
	}
}
