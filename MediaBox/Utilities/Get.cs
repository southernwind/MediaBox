using Prism.Ioc;
using Prism.Unity;

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
			return (App.Current as PrismApplication).Container.Resolve<T>();
		}
	}
}
