using System;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// パス設定
	/// </summary>
	public interface IPathSettings : ISettingsBase, IDisposable {
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
		/// FFmpegディレクトリパス
		/// </summary>
		SettingsItem<string> FFmpegDirectoryPath {
			get;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}