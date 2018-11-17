using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models.Settings;
using Unity;
using Unity.Lifetime;

namespace SandBeige.MediaBox.Repository {
	/// <summary>
	/// DIコンテナ登録クラス
	/// </summary>
	static class TypeRegistrations
    {
		public static void RegisterType(IUnityContainer unityContainer) {
			UnityConfig.UnityContainer = unityContainer;
			// ロガー
			unityContainer.RegisterType<ILogging, Logging>(new ContainerControlledLifetimeManager());

			// 設定
			unityContainer.RegisterType<ISettings, Settings>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IGeneralSettings, GeneralSettings>(new ContainerControlledLifetimeManager());
		}
    }
}
