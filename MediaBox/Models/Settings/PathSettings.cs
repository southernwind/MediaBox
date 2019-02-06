using System;
using System.IO;

using Livet;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	/// <summary>
	/// パス設定
	/// </summary>
	public class PathSettings : NotificationObject, IPathSettings {
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
		/// 設定ロード
		/// </summary>
		/// <param name="pathSettings">読み込み元設定</param>
		public void Load(IPathSettings pathSettings) {
			this.DataBaseFilePath.Value = pathSettings.DataBaseFilePath.Value;
			this.ThumbnailDirectoryPath.Value = pathSettings.ThumbnailDirectoryPath.Value;
		}

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
