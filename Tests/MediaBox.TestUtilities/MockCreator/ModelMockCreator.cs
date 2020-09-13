using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;

using Livet;

using Moq;

using Prism.Ioc;
using Prism.Services.Dialogs;
using Prism.Unity;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.About;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Box;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Container;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Editor;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.Plugin;
using SandBeige.MediaBox.Composition.Interfaces.Models.States;
using SandBeige.MediaBox.Composition.Interfaces.Models.Tool;
using SandBeige.MediaBox.Composition.Interfaces.Services;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Utilities;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.ViewModels;

using Unity;

namespace SandBeige.MediaBox.TestUtilities.MockCreator {
	public static class ModelMockCreator {
		private static readonly AppForTest _app;
		static ModelMockCreator() {
			_app = new AppForTest();
		}
		public static Mock<IAlbumContainer> CreateAlbumContainerMock() {
			var mock = new Mock<IAlbumContainer>();
			return mock;
		}

		public static Mock<IAlbumSelectorProvider> CreateAlbumSelectorProviderMock() {
			var mock = new Mock<IAlbumSelectorProvider>();
			var mainAlbumSelectorMock = CreateAlbumSelectorMock();
			mock.Setup(x => x.Create("main")).Returns(mainAlbumSelectorMock.Object);
			var editorAlbumSelectorMock = CreateAlbumSelectorMock();
			mock.Setup(x => x.Create("editor")).Returns(editorAlbumSelectorMock.Object);
			return mock;
		}

		public static Mock<IFilteringCondition> CreateFilteringConditionMock() {
			var mock = new Mock<IFilteringCondition>();
			mock.Setup(x => x.DisplayName).Returns(Rp("displayName"));
			mock.Setup(x => x.FilterItemObjects).Returns(Rorc<IFilterItemObject>());
			return mock;
		}

		public static Mock<IFilterDescriptionManager> CreateFilterDescriptionManagerMock() {
			var mock = new Mock<IFilterDescriptionManager>();
			mock.Setup(x => x.FilteringConditions).Returns(Rorc<IFilteringCondition>());
			var filteringConditionMock = CreateFilteringConditionMock();
			mock.Setup(x => x.CurrentFilteringCondition).Returns(Rp<IFilteringCondition?>(filteringConditionMock.Object));
			return mock;
		}

		public static Mock<ISortCondition> CreateSortConditionMock() {
			var mock = new Mock<ISortCondition>();
			mock.Setup(x => x.DisplayName).Returns(Rp("display name"));
			mock.Setup(x => x.SortItemCreators).Returns(Rorc<ISortItemCreator>());
			mock.Setup(x => x.CandidateSortItemCreators).Returns(Rc<ISortItemCreator>());
			return mock;
		}

		public static Mock<ISortDescriptionManager> CreateSortDescriptionManagerMock() {
			var mock = new Mock<ISortDescriptionManager>();
			var sortConditionMock = CreateSortConditionMock();
			mock.Setup(x => x.CurrentSortCondition).Returns(Rp<ISortCondition?>(sortConditionMock.Object));
			mock.Setup(x => x.SortConditions).Returns(Rorc<ISortCondition>());
			mock.Setup(x => x.Direction).Returns(Rp(ListSortDirection.Ascending));
			return mock;
		}

		public static Mock<IAlbumBox> CreateAlbumBoxMock() {
			var mock = new Mock<IAlbumBox>();
			return mock;
		}

		public static Mock<IAlbumSelectorFolderObject> CreateAlbumSelectorFolderObject() {
			var mock = new Mock<IAlbumSelectorFolderObject>();
			return mock;
		}

		public static Mock<IAlbumSelector> CreateAlbumSelectorMock() {
			var mock = new Mock<IAlbumSelector>();
			var filterDescriptionManagerMock = CreateFilterDescriptionManagerMock();
			var sortDescriptionManagerMock = CreateSortDescriptionManagerMock();
			var albumBoxMock = CreateAlbumBoxMock();
			var albumSelectorFolderObjectMock = CreateAlbumSelectorFolderObject();
			var albumMock = CreateAlbumModelMock();
			mock.Setup(x => x.Album).Returns(albumMock.Object);
			mock.Setup(x => x.FilterSetter).Returns(filterDescriptionManagerMock.Object);
			mock.Setup(x => x.SortSetter).Returns(sortDescriptionManagerMock.Object);
			mock.Setup(x => x.Shelf).Returns(Rp(albumBoxMock.Object));
			mock.Setup(x => x.Folder).Returns(Rp(albumSelectorFolderObjectMock.Object));
			return mock;
		}

		public static Mock<IAlbumForEditorModel> CreateAlbumForEditorModelMock() {
			var mock = new Mock<IAlbumForEditorModel>();
			mock.Setup(x => x.AlbumId).Returns(Rp(0));
			mock.Setup(x => x.AlbumBoxId).Returns(Rp<int?>(null));
			mock.Setup(x => x.Title).Returns(Rp("title"));
			mock.Setup(x => x.Directories).Returns(Rc<string>());
			return mock;
		}

		public static Mock<IStates> CreateStatesMock() {
			var mock = new Mock<IStates>();
			mock.Setup(x => x.AlbumStates).Returns(new AlbumStates());
			return mock;
		}

		public static Mock<IAlbumModel> CreateAlbumModelMock() {
			var mediaFileModelMock = CreateMediaFileModelMock();
			var mock = new Mock<IAlbumModel>();
			mock.Setup(x => x.Count).Returns(Rp(0));
			mock.Setup(x => x.Title).Returns(Rp("title"));
			mock.Setup(x => x.ResponseTime).Returns(Rp(0L));
			mock.Setup(x => x.BeforeFilteringCount).Returns(Rp(0));
			mock.Setup(x => x.ZoomLevel).Returns(Rp(0));
			mock.Setup(x => x.CurrentMediaFile).Returns(Rp<IMediaFileModel?>(mediaFileModelMock.Object));
			mock.Setup(x => x.CurrentIndex).Returns(Rp(0));
			mock.Setup(x => x.Items).Returns(new ObservableSynchronizedCollection<IMediaFileModel>());
			mock.Setup(x => x.CurrentMediaFiles).Returns(Rp(Array.Empty<IMediaFileModel>().AsEnumerable()));
			mock.Setup(x => x.CompositeDisposable).Returns(new CompositeDisposable());
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
			var licenseMock = CreateLicenseMock();
			var mock = new Mock<IAboutModel>();
			mock.Setup(x => x.Licenses).Returns(Rc<ILicense>());
			mock.Setup(x => x.CurrentLicense).Returns(Rp(licenseMock.Object));
			mock.Setup(x => x.LicenseText).Returns(Rp("license text"));
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
			mock.Object.GeneralSettings.ImageExtensions.AddRange(new[] { ".jpg", ".png" });
			mock.Object.GeneralSettings.VideoExtensions.Add(".mov");
			mock.Setup(x => x.PathSettings).Returns(new PathSettings());
			mock.Setup(x => x.PluginSettings).Returns(new PluginSettings());
			mock.Setup(x => x.ScanSettings).Returns(new ScanSettings());
			mock.Setup(x => x.ViewerSettings).Returns(new ViewerSettings());
			return mock;
		}

		public static Mock<IExternalToolsFactory> CreateExternalToolsFactoryMock() {
			var mock = new Mock<IExternalToolsFactory>();
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

		public static Mock<IChecker> CreateCheckerMock() {
			var mock = new Mock<IChecker>();
			return mock;
		}

		public static IFilterItemFactory CreateFilterItemFactory() {
			var container = CreateContainer();
			var settingsMock = CreateSettingsMock();
			container.RegisterInstance(settingsMock.Object);
			return new FilterItemFactory(container);
		}

		public static Mock<IFilterItem> CreateFilterItemMock() {
			var mock = new Mock<IFilterItem>();
			return mock;
		}

		public static UnityContainerExtension CreateContainer() {
			var unityContainer = new UnityContainer();
			var unityContainerExtension = new UnityContainerExtension(unityContainer);
			_app.Register(unityContainerExtension);
			return unityContainerExtension;
		}

		private static ReactivePropertySlim<T> Rp<T>(T initialValue) {
			return new ReactivePropertySlim<T>(initialValue);
		}

		private static ReactiveCollection<T> Rc<T>() {
			return new ReactiveCollection<T>();
		}

		private static ReadOnlyReactiveCollection<T> Rorc<T>() where T : class {
			return Rc<T>().ToReadOnlyReactiveCollection();
		}
	}

	internal class AppForTest : App {
		public void Register(IContainerRegistry containerRegistry) {
			base.RegisterRequiredTypes(containerRegistry);
		}
	}
}
