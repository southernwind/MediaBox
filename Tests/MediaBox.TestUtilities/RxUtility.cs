using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SandBeige.MediaBox.TestUtilities {
	public static class RxUtility {
		public static void WaitScheduler(IScheduler scheduler) {
			var are = new AutoResetEvent(false);
			scheduler.Schedule(() => {
				are.Set();
			});
			are.WaitOne();
		}

		/// <summary>
		/// 一定時間定期的に条件を確認しながら条件に合致するまで待機
		/// </summary>
		/// <param name="condition">条件</param>
		/// <param name="intervalMilliseconds">確認間隔</param>
		/// <param name="timeoutMilliseconds">タイムアウト</param>
		public static async Task WaitPolling(Func<bool> condition, int intervalMilliseconds, int timeoutMilliseconds) {
			await Observable
				.Interval(TimeSpan.FromMilliseconds(intervalMilliseconds))
				.Where(_ => condition())
				.Timeout(TimeSpan.FromMilliseconds(timeoutMilliseconds))
				.FirstAsync();
		}
	}
}
