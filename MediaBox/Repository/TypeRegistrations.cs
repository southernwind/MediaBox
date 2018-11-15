using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.God;
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
			unityContainer.RegisterType<ILogging, Logging>(new ContainerControlledLifetimeManager());
		}
    }
}
