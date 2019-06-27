using System;
using System.Threading;

namespace SandBeige.MediaBox.God {
	internal sealed class DisposableLock : ReaderWriterLockSlim {
		public DisposableLock() : base() {
		}

		public DisposableLock(LockRecursionPolicy recursionPolicy) : base(recursionPolicy) {
		}

		public IDisposable DisposableEnterReadLock() {
			this.EnterReadLock();
			return new DisposeObject(this.ExitReadLock);
		}

		public IDisposable DisposableEnterWriteLock() {
			this.EnterWriteLock();
			return new DisposeObject(this.ExitWriteLock);
		}

		private class DisposeObject : IDisposable {
			private readonly Action _disposeAction;
			public DisposeObject(Action disposeAction) {
				this._disposeAction = disposeAction;
			}

			public void Dispose() {
				this._disposeAction();
			}
		}
	}
}
