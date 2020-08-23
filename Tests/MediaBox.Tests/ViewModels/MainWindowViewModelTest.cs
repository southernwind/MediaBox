
using NUnit.Framework;

using SandBeige.MediaBox.TestUtilities.MockCreator;
using SandBeige.MediaBox.ViewModels;
namespace SandBeige.MediaBox.Tests.ViewModels {
	[TestFixture]
	internal class MainWindowViewModelTest : ViewModelTestClassBase {
		[Test]
		public void インスタンス生成() {
			var mediaFileManagerMock = ModelMockCreator.CreateMediaFileManagerMock();
			var albumSelectorProviderMock = ModelMockCreator.CreateAlbumSelectorProviderMock();

			var statesMock = ModelMockCreator.CreateStatesMock();
			var viewModelFactoryMock = ModelMockCreator.CreateViewModelFactoryMock();
			var loggingMock = ModelMockCreator.CreateLoggingMock();
			using var vm = new MainWindowViewModel(mediaFileManagerMock.Object, albumSelectorProviderMock.Object, viewModelFactoryMock.Object, statesMock.Object, loggingMock.Object);
		}
	}
}
