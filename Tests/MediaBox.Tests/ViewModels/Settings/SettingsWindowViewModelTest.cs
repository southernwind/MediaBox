using Moq;

using NUnit.Framework;

using Prism.Services.Dialogs;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;
using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.Settings;

namespace SandBeige.MediaBox.Tests.ViewModels.Settings {
	[TestFixture]
	internal class SettingsWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var settingsMock = new Mock<ISettings>();
			settingsMock.Setup(x => x.GeneralSettings).Returns(new GeneralSettings());
			settingsMock.Setup(x => x.PathSettings).Returns(new PathSettings());
			settingsMock.Setup(x => x.PluginSettings).Returns(new PluginSettings());
			settingsMock.Setup(x => x.ScanSettings).Returns(new ScanSettings());
			settingsMock.Setup(x => x.ViewerSettings).Returns(new ViewerSettings());

			var externalToolsFactoryMock = new Mock<IExternalToolsFactory>();
			var statesMock = new Mock<IStates>();
			var dialogServiceMock = new Mock<IDialogService>();
			var viewModelFactoryMock = new Mock<ViewModelFactory>(() => new ViewModelFactory(dialogServiceMock.Object, settingsMock.Object, externalToolsFactoryMock.Object, statesMock.Object));
			var pluginManagerMock = new Mock<IPluginManager>();
			var folderSelectionServiceMock = new Mock<IFolderSelectionDialogService>();
			pluginManagerMock.Setup(x => x.PluginList).Returns(new ReactiveCollection<IPluginModel>().ToReadOnlyReactiveCollection());
			using var _ = new SettingsWindowViewModel(settingsMock.Object, viewModelFactoryMock.Object, pluginManagerMock.Object, folderSelectionServiceMock.Object);
		}
	}
}
