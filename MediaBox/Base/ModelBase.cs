using System;
using Livet;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.DataBase;
using SandBeige.MediaBox.Utilities;
using Unity.Attributes;

namespace SandBeige.MediaBox.Base {
	internal class ModelBase : NotificationObject,IDisposable {
		private bool _disposed;
		private LivetCompositeDisposable _compositeDisposable;

		protected ModelBase(){
			this.Logging = Get.Instance<ILogging>();
			this.Settings = Get.Instance<ISettings>();
			this.DataBase = Get.Instance<MediaBoxDbContext>();
		}


		public LivetCompositeDisposable CompositeDisposable {
			get {
				return this._compositeDisposable ?? (this._compositeDisposable = new LivetCompositeDisposable());
			}
		}

		/// <summary>
		/// ロガー
		/// </summary>
		protected ILogging Logging { get; set; }

		/// <summary>
		/// 設定
		/// </summary>
		protected ISettings Settings { get; set; }

		/// <summary>
		/// データベース
		/// </summary>
		protected MediaBoxDbContext DataBase { get; set; }

		public void Dispose() {
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (!this._disposed) {
				if (disposing) {
					this._compositeDisposable?.Dispose();
				}
				this._disposed = true;
			}
		}
	}
}
