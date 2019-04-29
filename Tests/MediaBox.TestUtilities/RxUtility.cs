using System.Reactive.Concurrency;
using System.Threading;

namespace SandBeige.MediaBox.TestUtilities {
	public static class RxUtility {
		public static void WaitScheduler(IScheduler scheduler) {
			var are = new AutoResetEvent(false);
			scheduler.Schedule(() => {
				are.Set();
			});
			are.WaitOne();
		}
	}
}
