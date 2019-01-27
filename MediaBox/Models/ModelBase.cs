using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

using Livet;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Models.Media;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models {
	internal class ModelBase : NotificationObject, IDisposable {

		private LivetCompositeDisposable _compositeDisposable;
		private readonly Subject<Unit> _onDisposed = new Subject<Unit>();
		private readonly ConcurrentDictionary<string, object> _backingFields = new ConcurrentDictionary<string, object>();

		protected ModelBase() {
			// 具象クラスのコンストラクタで使用することもあるため、属性付与によるプロパティインジェクションでは生成タイミングが遅すぎる
			this.Logging = Get.Instance<ILogging>();
			this.Settings = Get.Instance<ISettings>();
			this.States = Get.Instance<States.States>();
			this.DataBase = Get.Instance<MediaBoxDbContext>();
			this.MediaFactory = Get.Instance<MediaFactory>();
#if DISPOSE_LOG
			this.OnDisposed.Subscribe(x => {
				this.Logging.Log($"[Disposed]{this}", LogLevel.Debug);
			});
#endif
		}

		public bool Disposed {
			get;
			private set;
		}

		public IObservable<Unit> OnDisposed {
			get {
				return this._onDisposed.AsObservable();
			}
		}

		public LivetCompositeDisposable CompositeDisposable {
			get {
				return this._compositeDisposable ?? (this._compositeDisposable = new LivetCompositeDisposable());
			}
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
		/// 状態
		/// </summary>
		protected States.States States {
			get;
		}

		protected MediaFactory MediaFactory {
			get;
		}

		/// <summary>
		/// データベース
		/// </summary>
		protected MediaBoxDbContext DataBase {
			get;
		}

		/// <summary>
		/// バッキングフィールドから値を取得(Boxingが発生するのでパフォーマンスが重要な場面では使わない)
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="member">メンバー名</param>
		/// <returns>値</returns>
		protected T GetValue<T>([CallerMemberName] string member = "") {
			return this.GetValue<T>(() => default, member);
		}

		/// <summary>
		/// バッキングフィールドから値を取得(Boxingが発生するのでパフォーマンスが重要な場面では使わない)
		/// </summary>
		/// <typeparam name="T">型</typeparam>
		/// <param name="valueFactory">バッキングフィールドに値がなかった場合の値生成関数</param>
		/// <param name="member">メンバー名</param>
		/// <returns>値</returns>
		protected T GetValue<T>(Func<T> valueFactory, [CallerMemberName] string member = "") {
			return
				(T)this
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

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (this.Disposed) {
				return;
			}

			this._onDisposed.OnNext(Unit.Default);
			if (disposing) {
				this._compositeDisposable?.Dispose();
			}
			this.Disposed = true;
		}
	}
}
