using System;
using System.Collections.Generic;
using SandBeige.MediaBox.Repository;
using Unity;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;

namespace SandBeige.MediaBox.Utilities {
	internal static class Get {
		/// <summary>
		/// DIコンテナ経由でインスタンスを取得する
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <returns>取得したインスタンス</returns>
		public static T Instance<T>() {
			return UnityConfig.UnityContainer.Resolve<T>();
		}

		/// <summary>
		/// DIコンテナ経由でインスタンスを取得する
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <returns>取得したインスタンス</returns>
		public static T Instance<T>(params object[] parameters) {
			return UnityConfig.UnityContainer.Resolve<T>(new ParamResolverOverride(parameters));
		}

		/// <summary>
		/// パラメータが必要なコンストラクタにパラメータを供給するためのクラス
		/// </summary>
		private class ParamResolverOverride : ResolverOverride {
			private readonly Queue<InjectionParameterValue> _parametersQueue;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="parameters"></param>
			public ParamResolverOverride(object[] parameters) {
				this._parametersQueue = new Queue<InjectionParameterValue>(InjectionParameterValue.ToParameters(parameters));
			}

			/// <summary>
			/// パラメータが要求されたときに、順番に一つずつ供給していく
			/// </summary>
			/// <param name="context"></param>
			/// <param name="dependencyType"></param>
			/// <returns>パラメータ</returns>
			public override IResolverPolicy GetResolver(IBuilderContext context, Type dependencyType) {
				if (this._parametersQueue.Count < 1) {
					return null;
				}

				var value = this._parametersQueue.Dequeue();
				return value.GetResolverPolicy(dependencyType);
			}
		}
	}
}
