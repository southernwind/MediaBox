using Moq;

using NUnit.Framework;

using Prism.Services.Dialogs;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.ViewModels;
namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class MainWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var mediaFileManagerMock = new Mock<IMediaFileManager>();
			var albumSelectorProviderMock = new Mock<IAlbumSelectorProvider>();
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
			var loggingMock = new Mock<ILogging>();
			using var vm = new MainWindowViewModel(mediaFileManagerMock.Object, albumSelectorProviderMock.Object, viewModelFactoryMock.Object, statesMock.Object, loggingMock.Object);
		}
	}
}
