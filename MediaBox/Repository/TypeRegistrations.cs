using System;
using System.IO;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Notification;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.Views.Map;

using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Repository {

	/// <summary>
	/// DIコンテナ登録クラス
	/// </summary>
	internal static class TypeRegistrations {
		/// <summary>
		/// DIコンテナに型を登録するメソッド
		/// </summary>
		/// <param name="unityContainer">コンテナインスタンス</param>
		public static void RegisterType(IUnityContainer unityContainer) {
			UnityConfig.UnityContainer = unityContainer;
			// ロガー
			unityContainer.RegisterType<ILogging, Logging>(new ContainerControlledLifetimeManager());

			// 設定
			unityContainer.RegisterType<ISettings, Settings>(
				new ContainerControlledLifetimeManager(),
				new InjectionConstructor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.settings"))
			);
			unityContainer.RegisterType<IGeneralSettings, GeneralSettings>();
			unityContainer.RegisterType<IPathSettings, PathSettings>();
			unityContainer.RegisterType<IScanSettings, ScanSettings>();
			unityContainer.RegisterType<States>(
				new ContainerControlledLifetimeManager(),
				new InjectionConstructor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.states"))
			);

			// Singleton
			unityContainer.RegisterType<AlbumContainer>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<MediaFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ViewModelFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ExternalToolsFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<PriorityTaskQueue>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<AlbumHistoryManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<MediaFileManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<GeoCodingManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<NotificationManager>(new ContainerControlledLifetimeManager());

			// Map
			unityContainer.RegisterType<IMapControl, MapControl>();
		}
	}
}
