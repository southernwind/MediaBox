using System;
using System.ComponentModel;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 一般設定
	/// </summary>
	public interface IGeneralSettings : INotifyPropertyChanged, IDisposable {

		/// <summary>
		/// 管理対象拡張子
		/// </summary>
		SettingsCollection<string> TargetExtensions {
			get;
		}

		/// <summary>
		/// Bing Map Api Key
		/// </summary>
		SettingsItem<string> BingMapApiKey {
			get;
		}

		/// <summary>
		/// サムネイル幅
		/// </summary>
		SettingsItem<int> ThumbnailWidth {
			get;
		}

		/// <summary>
		/// サムネイル高さ
		/// </summary>
		SettingsItem<int> ThumbnailHeight {
			get;
		}

		/// <summary>
		/// マップピンサイズ
		/// </summary>
		SettingsItem<int> MapPinSize {
			get;
		}

		/// <summary>
		/// 表示モード
		/// </summary>
		SettingsItem<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// ソート設定
		/// </summary>
		SettingsItem<SortDescriptionParams[]> SortDescriptions {
			get;
		}

		/// <summary>
		/// 外部ツール
		/// </summary>
		SettingsCollection<ExternalToolParams> ExternalTools {
			get;
		}

		/// <summary>
		/// 設定ロード
		/// </summary>
		/// <param name="generalSettings">読み込み元設定</param>
		void Load(IGeneralSettings generalSettings);

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}