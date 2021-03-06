using System;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 設定インターフェイス
	/// </summary>
	public interface ISettings : IDisposable {
		/// <summary>
		/// 一般設定
		/// </summary>
		IGeneralSettings GeneralSettings {
			get;
		}

		/// <summary>
		/// パス設定
		/// </summary>
		IPathSettings PathSettings {
			get;
		}

		/// <summary>
		/// スキャン設定
		/// </summary>
		IScanSettings ScanSettings {
			get;
		}

		/// <summary>
		/// 表示設定
		/// </summary>
		IViewerSettings ViewerSettings {
			get;
		}

		/// <summary>
		/// プラグイン設定
		/// </summary>
		IPluginSettings PluginSettings {
			get;
		}

		/// <summary>
		/// ファイルパス設定
		/// </summary>
		/// <param name="path">パス</param>
		void SetFilePath(string path);

		/// <summary>
		/// 保存
		/// </summary>
		void Save();

		/// <summary>
		/// 読み込み
		/// </summary>
		void Load();
	}
}
