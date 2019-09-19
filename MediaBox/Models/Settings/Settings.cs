using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xaml;
using System.Xml;

using Livet;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Settings {
	public class Settings : NotificationObject, ISettings {
		private readonly string _settingsFilePath;

		/// <summary>
		/// ロガー
		/// </summary>
		protected ILogging Logging {
			get;
			set;
		}

		/// <summary>
		/// 一般設定
		/// </summary>
		public IGeneralSettings GeneralSettings {
			get;
			set;
		}

		/// <summary>
		/// パス設定
		/// </summary>
		public IPathSettings PathSettings {
			get;
			set;
		}

		/// <summary>
		/// スキャン設定
		/// </summary>
		public IScanSettings ScanSettings {
			get;
			set;
		}

		/// <summary>
		/// 表示設定
		/// </summary>
		public IViewerSettings ViewerSettings {
			get;
			set;
		}

		/// <summary>
		/// 表示設定
		/// </summary>
		public IPluginSettings PluginSettings {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public Settings() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="path"></param>
		public Settings(string path) {
			this._settingsFilePath = path;
			this.Logging = Get.Instance<ILogging>();
			this.GeneralSettings = Get.Instance<IGeneralSettings>();
			this.PathSettings = Get.Instance<IPathSettings>();
			this.ScanSettings = Get.Instance<IScanSettings>();
			this.ViewerSettings = Get.Instance<IViewerSettings>();
			this.PluginSettings = Get.Instance<IPluginSettings>();
		}

		/// <summary>
		/// 保存
		/// </summary>
		public void Save() {
			using var ms = new MemoryStream();
			var d = new ISettingsBase[] {
				this.GeneralSettings,
				this.PathSettings,
				this.ScanSettings,
				this.ViewerSettings,
				this.PluginSettings
			}.ToDictionary(x => x.GetType(), x => x.Export());
			XamlServices.Save(ms, d);
			try {
				using var fs = File.Create(this._settingsFilePath);
				ms.WriteTo(fs);
			} catch (IOException ex) {
				this.Logging.Log("設定保存失敗", LogLevel.Warning, ex);
			}
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		public void Load() {
			this.LoadDefault();
			if (!File.Exists(this._settingsFilePath)) {
				this.Logging.Log("設定ファイルなし");
				return;
			}

			try {
				if (!(XamlServices.Load(this._settingsFilePath) is Dictionary<Type, Dictionary<string, dynamic>> settings)) {
					this.Logging.Log("設定ファイル読み込み失敗", LogLevel.Warning);
					return;
				}

				foreach (var s in new ISettingsBase[] { this.GeneralSettings, this.PathSettings, this.ScanSettings, this.ViewerSettings, this.PluginSettings }) {
					if (settings.TryGetValue(s.GetType(), out var d)) {
						s.Import(d);
					}
				}
			} catch (XmlException ex) {
				this.Logging.Log("設定ファイル読み込み失敗", LogLevel.Warning, ex);
			}
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		private void LoadDefault() {
			this.GeneralSettings.LoadDefault();
			this.PathSettings.LoadDefault();
			this.ScanSettings.LoadDefault();
			this.ViewerSettings.LoadDefault();
			this.PluginSettings.LoadDefault();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.GeneralSettings?.Dispose();
			this.PathSettings?.Dispose();
			this.ScanSettings?.Dispose();
			this.ViewerSettings?.Dispose();
			this.PluginSettings?.Dispose();
		}
	}
}
