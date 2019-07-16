
using System.Threading;

using NUnit.Framework;

using SandBeige.MediaBox.God;

namespace SandBeige.MediaBox.Tests.God {
	[TestFixture]
	internal class DisposableLockTest {
		[Test]
		public void Lock() {
			using var dl = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
			using (var rl = dl.DisposableEnterReadLock()) {

			}
			using var _ = dl.DisposableEnterWriteLock();
		}
	}
}
