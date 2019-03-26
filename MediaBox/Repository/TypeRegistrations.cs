﻿using System;
using System.IO;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.TaskQueue;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.About;
using SandBeige.MediaBox.ViewModels.Album;
using SandBeige.MediaBox.ViewModels.Album.Filter;
using SandBeige.MediaBox.ViewModels.Album.Sort;
using SandBeige.MediaBox.ViewModels.Settings;
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
			unityContainer.RegisterType<IForTestSettings, ForTestSettings>();
			unityContainer.RegisterType<IScanSettings, ScanSettings>();
			unityContainer.RegisterType<States>(
				new ContainerControlledLifetimeManager(),
				new InjectionConstructor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.states"))
			);

			// Singleton画面
			unityContainer.RegisterType<SettingsWindowViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<AboutWindowViewModel>(new ContainerControlledLifetimeManager());

			// Singleton
			unityContainer.RegisterType<AlbumContainer>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<MediaFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ViewModelFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ExternalToolsFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<SortDescriptionManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<SortDescriptionManagerViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<FilterDescriptionManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<FilterDescriptionManagerViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<PriorityTaskQueue>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<AlbumHistoryManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<MediaFileManager>(new ContainerControlledLifetimeManager());

			unityContainer.RegisterType<AlbumSelectorViewModel>("main", new ContainerControlledLifetimeManager());
			// Map
			unityContainer.RegisterType<IMapControl, MapControl>();

			unityContainer.RegisterType<IThumbnail, Thumbnail>();
		}
	}
}
