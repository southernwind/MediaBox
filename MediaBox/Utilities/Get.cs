using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Repository;

using Unity;
using Unity.Builder;
using Unity.Injection;
using Unity.Policy;
using Unity.Resolution;

namespace SandBeige.MediaBox.Utilities {
	/// <summary>
	/// 取得クラス
	/// </summary>
	internal static class Get {
#if DI_LOG
		private static readonly System.Reactive.Subjects.Subject<System.Reactive.Unit> _onGetInstance = new System.Reactive.Subjects.Subject<System.Reactive.Unit>();
		private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, Counter> _instanceCount = new System.Collections.Concurrent.ConcurrentDictionary<Type, Counter>();
		static Get() {
			System.Reactive.Linq.Observable.Sample(
				_onGetInstance, TimeSpan.FromSeconds(1))
				.Subscribe(_ => {
					Console.WriteLine(
						string.Join(
							", ",
							System.Linq.Enumerable.Select(
								System.Linq.Enumerable.Where(
									_instanceCount,
									x => x.Value.Count > 10),
							x => $"[{x.Key.Name}({x.Value.Count,5})]")
						)
					);
				});
		}

		private class Counter {
			private static readonly object _lockObj;
			public long Count {
				get;
				private set;
			}

			static Counter() {
				_lockObj = new object();
			}

			public void AddOne() {
				lock (_lockObj) {
					this.Count++;
				}
			}
		}
#endif
		/// <summary>
		/// DIコンテナ経由でインスタンスを取得する
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <returns>取得したインスタンス</returns>
		public static T Instance<T>() {
#if DI_LOG
			var type = _instanceCount.GetOrAdd(typeof(T), new Counter());
			type.AddOne();
			_onGetInstance.OnNext(System.Reactive.Unit.Default);
#endif
			return UnityConfig.UnityContainer.Resolve<T>();
		}

		/// <summary>
		/// DIコンテナ経由でインスタンスを取得する
		/// </summary>
		/// <typeparam name="T">取得する型</typeparam>
		/// <returns>取得したインスタンス</returns>
		public static T Instance<T>(params object[] parameters) {
#if DI_LOG
			var type = _instanceCount.GetOrAdd(typeof(T), new Counter());
			type.AddOne();
			_onGetInstance.OnNext(System.Reactive.Unit.Default);
#endif
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
