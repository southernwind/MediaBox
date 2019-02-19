﻿using System;
using System.IO;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models.Album;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Models.Settings;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Models.Tools;
using SandBeige.MediaBox.ViewModels;
using SandBeige.MediaBox.ViewModels.Album.Filter;

using Unity;
using Unity.Injection;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Repository {
	using SandBeige.MediaBox.ViewModels.Album.Sort;

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
			unityContainer.RegisterType<States>(
				new ContainerControlledLifetimeManager(),
				new InjectionConstructor(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.states"))
			);

			// Singleton
			unityContainer.RegisterType<AlbumContainer>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<MediaFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ViewModelFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ExternalToolsFactory>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<SortDescriptionManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<SortDescriptionManagerViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<FilterDescriptionManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<FilterDescriptionManagerViewModel>(new ContainerControlledLifetimeManager());
			// Map
			unityContainer.RegisterType<IMapControl, MapControl>();

			unityContainer.RegisterType<IThumbnail, Thumbnail>();
		}
	}
}
