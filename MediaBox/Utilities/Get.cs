using SandBeige.MediaBox.Repository;
using Unity;

namespace SandBeige.MediaBox.Utilities {
	static class Get {
		/// <summary>
		/// DIコンテナ経由でインスタンスを取得する
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <returns>取得したインスタンス</returns>
		public static T Instance<T>() {
			return UnityConfig.UnityContainer.Resolve<T>();
		}
	}
}
