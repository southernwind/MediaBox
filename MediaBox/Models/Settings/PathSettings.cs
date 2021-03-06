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
		} = new SettingsItem<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "MediaBox.db"));

		/// <summary>
		/// サムネイルディレクトリパス
		/// </summary>
		public SettingsItem<string> ThumbnailDirectoryPath {
			get;
			set;
		} = new SettingsItem<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, "thumbs"));

		/// <summary>
		/// Ffmpegディレクトリパス
		/// </summary>
		public SettingsItem<string> FfmpegDirectoryPath {
			get;
			set;
		} = new SettingsItem<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, @"Externals\ffmpeg"));

		/// <summary>
		/// プラグインディレクトリパス
		/// </summary>
		public SettingsItem<string> PluginDirectoryPath {
			get;
			set;
		} = new SettingsItem<string>(Path.Combine(AppDomain.CurrentDomain.BaseDirectory!, @"Plugins"));
	}
}
