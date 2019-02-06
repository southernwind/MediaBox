using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace SandBeige.MediaBox.Library.Extensions {
	/// <summary>
	/// <see cref="IObservable{T}"/>の拡張メソッドクラス
	/// </summary>
	public static class ObservableEx {
		/// <summary>
		/// バックグラウンド実行
		/// テスト時は<paramref name="runOnBackground"/>をfalseにしてフォアグラウンドで実行してテストを行う。
		/// 通常実行時はtrueなのでバックグラウンドで処理される。
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="source"></param>
		/// <param name="runOnBackground">バックグラウンドで実行するか否か しない場合は<paramref name="source"/>がそのまま返される</param>
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
