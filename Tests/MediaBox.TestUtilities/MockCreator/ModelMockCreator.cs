
using Moq;

using Prism.Services.Dialogs;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.About;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Editor;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;
using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.ViewModels;

namespace SandBeige.MediaBox.TestUtilities.MockCreator {
	public static class ModelMockCreator {
		public static Mock<IAlbumContainer> CreateAlbumContainerMock() {
			var mock = new Mock<IAlbumContainer>();
			return mock;
		}

		public static Mock<IAlbumSelectorProvider> CreateAlbumSelectorProviderMock() {
			var mock = new Mock<IAlbumSelectorProvider>();
			var editorAlbumSelectorMock = CreateAlbumSelectorMock();
			mock.Setup(x => x.Create("editor")).Returns(editorAlbumSelectorMock.Object);
			return mock;
		}

		public static Mock<IAlbumSelector> CreateAlbumSelectorMock() {
			var mock = new Mock<IAlbumSelector>();
			return mock;
		}

		public static Mock<IAlbumForEditorModel> CreateAlbumForEditorModelMock() {
			var mock = new Mock<IAlbumForEditorModel>();
			mock.Setup(x => x.AlbumId).Returns(new ReactiveProperty<int>());
			mock.Setup(x => x.AlbumBoxId).Returns(new ReactiveProperty<int?>());
			mock.Setup(x => x.Title).Returns(new ReactiveProperty<string>());
			mock.Setup(x => x.Directories).Returns(new ReactiveCollection<string>());
			return mock;
		}

		public static Mock<IStates> CreateStatesMock() {
			var mock = new Mock<IStates>();
			mock.Setup(x => x.AlbumStates).Returns(new AlbumStates());
			return mock;
		}

		public static Mock<IAlbumObject> CreateAlbumObjectMock() {
			var mock = new Mock<IAlbumObject>();
			return mock;
		}

		public static Mock<IEditableAlbumObject> CreateEditableAlbumObjectMock() {
			var mock = new Mock<IEditableAlbumObject>();
			return mock;
		}

		public static Mock<IMediaFileModel> CreateMediaFileModelMock() {
			var mock = new Mock<IMediaFileModel>();
			return mock;
		}

		public static Mock<IAboutModel> CreateAboutModelMock() {
			var mock = new Mock<IAboutModel>();
			mock.Setup(x => x.Licenses).Returns(new ReactiveCollection<ILicense>());
			mock.Setup(x => x.CurrentLicense).Returns(new ReactivePropertySlim<ILicense>());
			mock.Setup(x => x.LicenseText).Returns(new ReactivePropertySlim<string>());
			return mock;
		}

		public static Mock<ILicense> CreateLicenseMock() {
			var mock = new Mock<ILicense>();
			return mock;
		}

		public static Mock<ILogging> CreateLoggingMock() {
			var mock = new Mock<ILogging>();
			return mock;
		}

		public static Mock<PriorityTaskQueue> CreatePriorityTaskQueueMock() {
			var loggingMock = CreateLoggingMock();
			var mock = new Mock<PriorityTaskQueue>(() => new PriorityTaskQueue(loggingMock.Object));
			return mock;
		}

		public static Mock<NotificationManager> CreateNotificationManagerMock() {
			var mock = new Mock<NotificationManager>();
			return mock;
		}

		public static Mock<IMediaFileManager> CreateMediaFileManagerMock() {
			var mock = new Mock<IMediaFileManager>();
			return mock;
		}

		public static Mock<ISettings> CreateSettingsMock() {
			var mock = new Mock<ISettings>();
			mock.Setup(x => x.GeneralSettings).Returns(new GeneralSettings());
			mock.Setup(x => x.PathSettings).Returns(new PathSettings());
			mock.Setup(x => x.PluginSettings).Returns(new PluginSettings());
			mock.Setup(x => x.ScanSettings).Returns(new ScanSettings());
			mock.Setup(x => x.ViewerSettings).Returns(new ViewerSettings());
			return mock;
		}

		public static Mock<IExternalToolsFactory> CreateExternalToolsFactoryMock() {
			var mock = new Mock<IExternalToolsFactory>();
			;
			return mock;
		}

		public static Mock<IDialogService> CreateDialogServiceMock() {
			var mock = new Mock<IDialogService>();
			return mock;
		}

		public static Mock<ViewModelFactory> CreateViewModelFactoryMock() {
			var externalToolsFactoryMock = CreateExternalToolsFactoryMock();
			var statesMock = CreateStatesMock();
			var dialogServiceMock = CreateDialogServiceMock();
			var settingsMock = CreateSettingsMock();
			var mock = new Mock<ViewModelFactory>(() => new ViewModelFactory(dialogServiceMock.Object, settingsMock.Object, externalToolsFactoryMock.Object, statesMock.Object));
			return mock;
		}

		public static Mock<IPluginManager> CreatePluginManagerMock() {
			var mock = new Mock<IPluginManager>();
			return mock;
		}

		public static Mock<IFolderSelectionDialogService> CreateFolderSelectionDialogServiceMock() {
			var mock = new Mock<IFolderSelectionDialogService>();
			return mock;
		}
	}
}
