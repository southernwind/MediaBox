using System;
using System.Threading;

namespace SandBeige.MediaBox.God {
	internal sealed class DisposableLock : ReaderWriterLockSlim {
		private bool _disposed;
		public DisposableLock() : base() {
		}

		public DisposableLock(LockRecursionPolicy recursionPolicy) : base(recursionPolicy) {
		}

		public IDisposable DisposableEnterReadLock() {
			if (this._disposed) {
				return new DisposeObject(null);
			}
			this.EnterReadLock();
			return new DisposeObject(this.ExitReadLock);
		}

		public IDisposable DisposableEnterWriteLock() {
			if (this._disposed) {
				return new DisposeObject(null);
			}
			this.EnterWriteLock();
			return new DisposeObject(this.ExitWriteLock);
		}

		public new void Dispose() {
			this._disposed = true;
			base.Dispose();
		}

		private class DisposeObject : IDisposable {
			private readonly Action _disposeAction;
			public DisposeObject(Action disposeAction) {
				this._disposeAction = disposeAction;
			}

			public void Dispose() {
				this._disposeAction?.Invoke();
			}
		}
	}
}
