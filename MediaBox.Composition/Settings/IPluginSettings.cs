using System;

using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// プラグイン設定
	/// </summary>
	public interface IPluginSettings : ISettingsBase, IDisposable {

		/// <summary>
		/// プラグインリスト
		/// </summary>
		SettingsCollection<PluginItem> PluginList {
			get;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}