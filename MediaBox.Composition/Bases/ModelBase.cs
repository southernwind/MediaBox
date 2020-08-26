using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;

using Prism.Mvvm;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.God;
using SandBeige.MediaBox.Composition.Interfaces.Models;

namespace SandBeige.MediaBox.Composition.Bases {
	/// <summary>
	/// モデル基底クラス
	/// </summary>
	public class ModelBase : BindableBase, IModelBase {
		/// <summary>
		/// Dispose用Lockオブジェクト
		/// 処理を行っている途中でDisposeされるとマズイ場合、このオブジェクトでロックしておく。
		/// </summary>
		protected readonly DisposableLock DisposeLock = new DisposableLock(LockRecursionPolicy.SupportsRecursion);
		/// <summary>
		/// まとめてDispose
		/// </summary>
		private CompositeDisposable? _compositeDisposable;

		/// <summary>
		/// Dispose通知用Subject
		/// </summary>
		private readonly Subject<Unit> _onDisposed = new Subject<Unit>();

		/// <summary>
		/// バッキングフィールド
		/// </summary>
		private readonly ConcurrentDictionary<string, object?> _backingFields = new();


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
		/// バッキングフィールドから値を取得(Boxingが発生するのでパフォーマンスが重要な場面では使わない)
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="member">メンバー名</param>
		/// <returns>値</returns>
		protected T? GetValue<T>([CallerMemberName] string member = "") {
			return this.GetValue<T?>(() => default, member);
		}

		/// <summary>
		/// バッキングフィールドから値を取得(Boxingが発生するのでパフォーマンスが重要な場面では使わない)
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="valueFactory">バッキングフィールドに値がなかった場合の値生成関数</param>
		/// <param name="member">メンバー名</param>
		/// <returns>値</returns>
		protected T? GetValue<T>(Func<T> valueFactory, [CallerMemberName] string member = "") {
			return
				(T?)this
					._backingFields
					.GetOrAdd(member, _ => valueFactory());
		}

		/// <summary>
		/// バッキングフィールドに値を設定(Boxingが発生するのでパフォーマンスが重要な場面では使わない)
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="value">値</param>
		/// <param name="member">メンバー名</param>
		protected void SetValue<T>(T value, [CallerMemberName] string member = "") {
			if (EqualityComparer<T>.Default.Equals(this.GetValue<T>(member), value)) {
				return;
			}
			this._backingFields[member] = value;
			this.RaisePropertyChanged(member);
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
	}
}
