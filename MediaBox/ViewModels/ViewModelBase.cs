using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

using Livet;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.God;
using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels {
	/// <summary>
	/// ViewModel基底クラス
	/// </summary>
	internal class ViewModelBase : ViewModel {
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
		/// ViewModelファクトリー
		/// </summary>
		protected ViewModelFactory ViewModelFactory {
			get;
		}

		/// <summary>
		/// ToStringするときに使用するモデルインスタンス
		/// </summary>
		protected ModelBase ModelForToString {
			get;
			set;
		}

		/// <summary>
		/// ロガー
		/// </summary>
		protected ILogging Logging {
			get;
		}

		/// <summary>
		/// 設定
		/// </summary>
		protected ISettings Settings {
			get;
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
		/// 状態
		/// </summary>
		public States States {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected ViewModelBase() {
			// 具象クラスのコンストラクタで使用することもあるため、属性付与によるプロパティインジェクションでは生成タイミングが遅すぎる
			this.Logging = Get.Instance<ILogging>();
			this.Settings = Get.Instance<ISettings>();
			this.States = Get.Instance<States>();
			this.ViewModelFactory = Get.Instance<ViewModelFactory>();
#if DISPOSE_LOG
			this.OnDisposed.Subscribe(x => {
				this.Logging.Log($"[Disposed]{this}", LogLevel.Debug);
			});
#endif
		}

		/// <summary>
		/// Dispose
		/// </summary>
		/// <param name="disposing">マネージドリソースの破棄を行うかどうか</param>
		protected override void Dispose(bool disposing) {
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
					base.Dispose(disposing);
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
