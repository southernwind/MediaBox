using System;
using System.Threading;

namespace SandBeige.MediaBox.God {
	/// <summary>
	/// リソースへのアクセス管理に使用するロックを表し、複数のスレッドによる読み取りや排他アクセスでの書き込みを実現します。
	/// ロック時に返却されるオブジェクトを破棄することで、ロックの解除が行えます。
	/// </summary>
	internal sealed class DisposableLock : ReaderWriterLockSlim {
		private bool _disposed;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="recursionPolicy">同じスレッドが複数回ロックに入ることができるかどうかを指定します。</param>
		public DisposableLock(LockRecursionPolicy recursionPolicy) : base(recursionPolicy) {
		}

		/// <summary>
		/// 読み取りモードでロックに入ることを試みます。
		/// </summary>
		/// <returns></returns>
		public IDisposable DisposableEnterReadLock() {
			if (this._disposed) {
				return new DisposeObject(null);
			}
			base.EnterReadLock();
			return new DisposeObject(this.ExitReadLock);
		}

		/// <summary>
		/// 書き込みモードでロックに入ることを試みます。
		/// </summary>
		/// <returns></returns>
		public IDisposable DisposableEnterWriteLock() {
			if (this._disposed) {
				return new DisposeObject(null);
			}
			base.EnterWriteLock();
			return new DisposeObject(this.ExitWriteLock);
		}

		/// <summary>
		/// <see cref="DisposableEnterReadLock"/>を使用すること
		/// </summary>
		[Obsolete]
		public new void EnterReadLock() {
			throw new NotSupportedException();
		}

		/// <summary>
		/// <see cref="DisposableEnterWriteLock"/>を使用すること
		/// </summary>
		[Obsolete]
		public new void EnterWriteLock() {
			throw new NotSupportedException();
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public new void Dispose() {
			this._disposed = true;
			base.Dispose();
		}

		/// <summary>
		/// ロック解除用オブジェクト
		/// </summary>
		private class DisposeObject : IDisposable {
			private readonly Action _disposeAction;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="disposeAction">Dispose時のアクション(ロック解除)</param>
			public DisposeObject(Action disposeAction) {
				this._disposeAction = disposeAction;
			}

			/// <summary>
			/// Dispose
			/// </summary>
			public void Dispose() {
				this._disposeAction?.Invoke();
			}
		}
	}
}
