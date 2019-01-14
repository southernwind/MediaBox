using System;
using System.IO;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.ViewModels;

using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Repository {
	/// <summary>
	/// DIコンテナ登録クラス
	/// </summary>
	internal static class TypeRegistrations {
		public static void RegisterType(IUnityContainer unityContainer) {
			UnityConfig.UnityContainer = unityContainer;
			// ロガー
			unityContainer.RegisterType<ILogging, Logging>(new ContainerControlledLifetimeManager());

			// 設定
			unityContainer.RegisterType<ISettings, Settings>(
				new ContainerControlledLifetimeManager(),
				new InjectionConstructor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.settings"))
			);
			unityContainer.RegisterType<IGeneralSettings, GeneralSettings>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IPathSettings, PathSettings>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IForTestSettings, ForTestSettings>(new ContainerControlledLifetimeManager());

			// Singleton
			unityContainer.RegisterType<AlbumContainer>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ThumbnailPool>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<MediaFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ViewModelFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<SortDescriptionManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<FilterDescriptionManager>(new ContainerControlledLifetimeManager());
			// Map
			unityContainer.RegisterType<IMapControl, MapControl>();

		}
	}
}
