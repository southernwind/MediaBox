using System;

using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	/// <summary>
	/// プラグイン設定
	/// </summary>
	public class PluginSettings : SettingsBase, IPluginSettings {

		/// <summary>
		/// プラグインリスト
		/// </summary>

		public SettingsCollection<PluginItem> PluginList {
			get;
		} = new SettingsCollection<PluginItem>(Array.Empty<PluginItem>());
	}
}
