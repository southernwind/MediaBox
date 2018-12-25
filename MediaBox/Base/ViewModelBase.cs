using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Livet;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Base {
	internal class ViewModelBase : ViewModel {
		private readonly Subject<Unit> _onDisposed = new Subject<Unit>();
		protected ViewModelBase() {
			this.Logging = Get.Instance<ILogging>();
			this.Settings = Get.Instance<ISettings>();
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

		protected override void Dispose(bool disposing) {
			if (this.Disposed) {
				return;
			}

			this._onDisposed.OnNext(Unit.Default);
			base.Dispose(disposing);
			this.Disposed = true;
		}
	}
}
