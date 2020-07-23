using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

using Prism.Mvvm;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// ViewModel基底クラス
	/// </summary>
	public class ViewModelBase : BindableBase, IDisposable {
		/// <summary>
		/// Dispose用Lockオブジェクト
		/// 処理を行っている途中でDisposeされるとマズイ場合、このオブジェクトでロックしておく。
		/// </summary>
		protected readonly DisposableLock DisposeLock = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
		/// <summary>
		/// Dispose通知用Subject
		/// </summary>
		private readonly Subject<Unit> _onDisposed = new Subject<Unit>();

		/// <summary>
		/// まとめてDispose
		/// </summary>
		private CompositeDisposable _compositeDisposable;

		/// <summary>
		/// ToStringするときに使用するモデルインスタンス
		/// </summary>
		protected ModelBase ModelForToString {
			get;
			set;
		}

		/// <summary>
		/// Dispose済みか
		/// </summary>
		public DisposeState DisposeState {
			get;
			private set;
		}

		/// <summary>
		/// Dispose通知
		/// </summary>
		public IObservable<Unit> OnDisposed {
			get {
				return this._onDisposed.AsObservable();
			}
		}

		/// <summary>
		/// まとめてDispose
		/// </summary>
		public CompositeDisposable CompositeDisposable {
			get {
				return this._compositeDisposable ??= new CompositeDisposable();
			}
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing">マネージドリソースの破棄を行うかどうか</param>
		protected virtual void Dispose(bool disposing) {
			lock (this.DisposeLock) {
				if (this.DisposeState != DisposeState.NotDisposed) {
					return;
				}
				using (this.DisposeLock.DisposableEnterWriteLock()) {
					if (this.DisposeState != DisposeState.NotDisposed) {
						return;
					}
					this.DisposeState = DisposeState.Disposing;
				}
				if (disposing) {
					this._onDisposed.OnNext(Unit.Default);
					this._compositeDisposable?.Dispose();
				}
				using (this.DisposeLock.DisposableEnterWriteLock()) {
					this.DisposeState = DisposeState.Disposed;
				}
				this.DisposeLock.Dispose();
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.ModelForToString}>";
		}
	}
}
