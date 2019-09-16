using System;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 表示設定
	/// </summary>
	public interface IViewerSettings : ISettingsBase, IDisposable {

		/// <summary>
		/// MediaFile表示Xaml
		/// </summary>
		SettingsItem<string> MediaFileViewerControlXaml {
			get;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}