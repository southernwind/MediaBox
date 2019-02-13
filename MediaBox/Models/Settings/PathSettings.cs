using System;
using System.IO;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	/// <summary>
	/// パス設定
	/// </summary>
	public class PathSettings : SettingsBase, IPathSettings {
		/// <summary>
		/// データベースファイルパス
		/// </summary>
		public SettingsItem<string> DataBaseFilePath {
			get;
			set;
		} = new SettingsItem<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MediaBox.db"));

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		public SettingsItem<string> ThumbnailDirectoryPath {
			get;
			set;
		} = new SettingsItem<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "thumbs"));

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		public void LoadDefault() {
			this.DataBaseFilePath.SetDefaultValue();
			this.ThumbnailDirectoryPath.SetDefaultValue();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.DataBaseFilePath?.Dispose();
			this.ThumbnailDirectoryPath?.Dispose();
		}

	}
}
