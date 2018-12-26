using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Livet;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models {
	internal class ModelBase : NotificationObject, IDisposable {

		private LivetCompositeDisposable _compositeDisposable;
		private readonly Subject<Unit> _onDisposed = new Subject<Unit>();

		protected ModelBase() {
			this.Logging = Get.Instance<ILogging>();
			this.Settings = Get.Instance<ISettings>();
			this.DataBase = Get.Instance<MediaBoxDbContext>();
		}
		public bool Disposed {
			private get;
			set;
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
		/// データベース
		/// </summary>
		protected MediaBoxDbContext DataBase {
			get;
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
