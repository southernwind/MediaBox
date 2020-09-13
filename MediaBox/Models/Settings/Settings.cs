using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xaml;
using System.Xml;

using Prism.Mvvm;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	public class Settings : BindableBase, ISettings {
		private string? _settingsFilePath;

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
			this.Logging = null!;
			this.GeneralSettings = null!;
			this.PathSettings = null!;
			this.ScanSettings = null!;
			this.ViewerSettings = null!;
			this.PluginSettings = null!;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Settings(ILogging logging, IGeneralSettings generalSettings, IPathSettings pathSettings, IScanSettings scanSettings, IViewerSettings viewerSettings, IPluginSettings pluginSettings) {
			this.Logging = logging;
			this.GeneralSettings = generalSettings;
			this.PathSettings = pathSettings;
			this.ScanSettings = scanSettings;
			this.ViewerSettings = viewerSettings;
			this.PluginSettings = pluginSettings;
		}

		/// <summary>
		/// ファイルパス設定
		/// </summary>
		/// <param name="path">パス</param>
		public void SetFilePath(string path) {
			this._settingsFilePath = path;
		}

		/// <summary>
		/// 保存
		/// </summary>
		public void Save() {
			if (this._settingsFilePath is null) {
				throw new InvalidOperationException();
			}
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
			if (this._settingsFilePath is null) {
				throw new InvalidOperationException();
			}
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
			this.GeneralSettings.Dispose();
			this.PathSettings.Dispose();
			this.ScanSettings.Dispose();
			this.ViewerSettings.Dispose();
			this.PluginSettings.Dispose();
		}
	}
}
