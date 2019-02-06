using System;
using System.ComponentModel;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// パス設定
	/// </summary>
	public interface IPathSettings : INotifyPropertyChanged, IDisposable {
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		SettingsItem<string> DataBaseFilePath {
			get;
		}

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		SettingsItem<string> ThumbnailDirectoryPath {
			get;
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="pathSettings">読み込み元設定</param>
		void Load(IPathSettings pathSettings);

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}