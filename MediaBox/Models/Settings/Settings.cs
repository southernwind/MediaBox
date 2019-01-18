using System;
using System.IO;
using System.Xaml;
using System.Xml;

using Livet;

using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;

using Unity.Attributes;

namespace SandBeige.MediaBox.Models.Settings {
	public class Settings : NotificationObject, ISettings {
		private readonly string _settingsFilePath;

		/// <summary>
		/// ロガー
		/// </summary>
		[Dependency]
		protected ILogging Logging {
			get;
			set;
		}

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

		/// <summary>
		/// テスト用設定
		/// </summary>
		[Dependency]
		public IForTestSettings ForTestSettings {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public Settings() {
		}

		public Settings(string path) {
			this._settingsFilePath = path;
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
				this.LoadDefault();
				return;
			}

			try {

				if (!(XamlServices.Load(this._settingsFilePath) is Settings settings)) {
					this.Logging.Log("設定ファイル読み込み失敗", LogLevel.Warning);
					this.LoadDefault();
					return;
				}

				this.GeneralSettings.Load(settings.GeneralSettings);
				this.PathSettings.Load(settings.PathSettings);
				this.ForTestSettings.Load(settings.ForTestSettings);
			} catch (XmlException ex) {
				this.Logging.Log("設定ファイル読み込み失敗", LogLevel.Warning, ex);
				this.LoadDefault();
			}
		}

		private void LoadDefault() {
			this.GeneralSettings.LoadDefault();
			this.PathSettings.LoadDefault();
			this.ForTestSettings.LoadDefault();
		}

		public void Dispose() {
			this.GeneralSettings?.Dispose();
			this.PathSettings?.Dispose();
			this.ForTestSettings?.Dispose();
		}
	}
}
