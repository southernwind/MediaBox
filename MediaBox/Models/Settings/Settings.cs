using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xaml;
using Reactive.Bindings.Extensions;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using Unity.Attributes;

namespace SandBeige.MediaBox.Models.Settings {
	public class Settings : ISettings {
		private readonly string _settingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.settings");
		private readonly CompositeDisposable _disposable = new CompositeDisposable();

		/// <summary>
		/// ロガー
		/// </summary>
		[Dependency]
		protected ILogging Logging { get; set; }

		/// <summary>
		/// 一般設定
		/// </summary>
		[Dependency]
		public IGeneralSettings GeneralSettings {
			get;
			set;
		}

		/// <summary>
		/// パス設定
		/// </summary>
		[Dependency]
		public IPathSettings PathSettings {
			get;
			set;
		}

		public Settings() {
		}

		/// <summary>
		/// 保存
		/// </summary>
		public void Save() {
			using (var ms = new MemoryStream()) {
				XamlServices.Save(ms, this);
				using (var fs = File.Create(this._settingsFilePath)) {
					ms.WriteTo(fs);
				}
			}
		}

		/// <summary>
		/// 読み込み
		/// </summary>
		public void Load() {
			if (!File.Exists(this._settingsFilePath)) {
				this.Logging.Log("設定ファイルなし");
				return;
			}
			if (!(XamlServices.Load(this._settingsFilePath) is Settings settings)) {
				this.Logging.Log("設定ファイル読み込み失敗", LogLevel.Warning);
				return;
			}
			this.GeneralSettings?.Dispose();
			this.GeneralSettings = settings.GeneralSettings.AddTo(this._disposable);
			this.PathSettings?.Dispose();
			this.PathSettings = settings.PathSettings.AddTo(this._disposable);
		}

		public void Dispose() {
			this._disposable.Dispose();
		}
	}
}
