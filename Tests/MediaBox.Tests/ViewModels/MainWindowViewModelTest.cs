using Moq;

using NUnit.Framework;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.ViewModels;
namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class MainWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var mediaFileManagerMock = new Mock<IMediaFileManager>();
			var albumSelectorProviderMock = new Mock<IAlbumSelectorProvider>();
			var viewModelFactoryMock = new Mock<ViewModelFactory>();
			var statesMock = new Mock<IStates>();
			var loggingMock = new Mock<ILogging>();
			using var vm = new MainWindowViewModel(mediaFileManagerMock.Object, albumSelectorProviderMock.Object, viewModelFactoryMock.Object, statesMock.Object, loggingMock.Object);
		}
	}
}
