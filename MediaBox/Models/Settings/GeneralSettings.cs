
using System;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : SettingsBase, IGeneralSettings {
		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		public SettingsCollection<string> TargetExtensions {
			get;
			set;
		} = new SettingsCollection<string>(".jpg", ".jpeg", ".png", ".mov");

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		public SettingsItem<string> BingMapApiKey {
			get;
			set;
		} = new SettingsItem<string>("");

		/// <summary>
		/// サムネイル幅
		/// </summary>
		public SettingsItem<int> ThumbnailWidth {
			get;
			set;
		} = new SettingsItem<int>(200);

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		public SettingsItem<int> ThumbnailHeight {
			get;
			set;
		} = new SettingsItem<int>(200);

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		public SettingsItem<int> MapPinSize {
			get;
			set;
		} = new SettingsItem<int>(150);

		/// <summary>
		/// 表示モード
		/// </summary>
		public SettingsItem<DisplayMode> DisplayMode {
			get;
			set;
		} = new SettingsItem<DisplayMode>(Composition.Enum.DisplayMode.Library);

		/// <summary>
		/// ソート設定
		/// </summary>
		public SettingsItem<SortDescriptionParams[]> SortDescriptions {
			get;
			set;
		} = new SettingsItem<SortDescriptionParams[]>(Array.Empty<SortDescriptionParams>());

		/// <summary>
		/// 外部ツール
		/// </summary>
		public SettingsCollection<ExternalToolParams> ExternalTools {
			get;
		} = new SettingsCollection<ExternalToolParams>(Array.Empty<ExternalToolParams>());

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		public void LoadDefault() {
			this.TargetExtensions.SetDefaultValue();
			this.BingMapApiKey.SetDefaultValue();
			this.ThumbnailWidth.SetDefaultValue();
			this.ThumbnailHeight.SetDefaultValue();
			this.MapPinSize.SetDefaultValue();
			this.DisplayMode.SetDefaultValue();
			this.SortDescriptions.SetDefaultValue();
			this.ExternalTools.SetDefaultValue();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.TargetExtensions?.Dispose();
			this.BingMapApiKey?.Dispose();
			this.ThumbnailWidth?.Dispose();
			this.ThumbnailHeight?.Dispose();
			this.MapPinSize?.Dispose();
			this.DisplayMode?.Dispose();
			this.SortDescriptions?.Dispose();
			this.ExternalTools?.Dispose();
		}
	}
}
