using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace SandBeige.MediaBox.Library.Extensions {
	public static class ObservableEx {
		public static IObservable<OldAndNewValue<T>> ToOldAndNewValue<T>(this IObservable<T> p) {
			return p.Zip(p.Skip(1), (x, y) => new OldAndNewValue<T>(x, y));
		}

		public class OldAndNewValue<T> {
			public OldAndNewValue(T oldValue, T newValue) {
				this.OldValue = oldValue;
				this.NewValue = newValue;
			}

			public T OldValue {
				get;
			}

			public T NewValue {
				get;
			}
		}

		/// <summary>
		/// バックグラウンド実行
		/// テスト時は<paramref name="runOnBackground"/>をfalseにしてフォアグラウンドで実行してテストを行う。
		/// 通常実行時はtrueなのでバックグラウンドで処理される。
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="source"></param>
		/// <param name="background">バックグラウンドで実行するか否か しない場合は<paramref name="source"/>がそのまま返される</param>
		/// <returns></returns>
		public static IObservable<T> ObserveOnBackground<T>(this IObservable<T> source, bool runOnBackground) {
			if (!runOnBackground) {
				return source;
			}

			return source
				.ObserveOn(Dispatcher.CurrentDispatcher, DispatcherPriority.Background)
				.ObserveOn(TaskPoolScheduler.Default);
		}
	}
}
