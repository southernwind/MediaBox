using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Livet;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Models;
using SandBeige.MediaBox.Models.States;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.ViewModels {
	internal class ViewModelBase : ViewModel {
		private readonly Subject<Unit> _onDisposed = new Subject<Unit>();
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

		public bool Disposed {
			get;
			private set;
		}

		public IObservable<Unit> OnDisposed {
			get {
				return this._onDisposed.AsObservable();
			}
		}


		protected ViewModelFactory ViewModelFactory {
			get;
		}

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
		/// 状態
		/// </summary>
		protected States States {
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

		public override string ToString() {
			return $"<[{base.ToString()}] {this.ModelForToString}>";
		}
	}
}
