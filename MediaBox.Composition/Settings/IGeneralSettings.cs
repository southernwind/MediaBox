using System;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Composition.Settings {
	/// <summary>
	/// 一般設定
	/// </summary>
	public interface IGeneralSettings : ISettingsBase, IDisposable {

		/// <summary>
		/// 画像拡張子
		/// </summary>
		SettingsCollection<string> ImageExtensions {
			get;
		}

		/// <summary>
		/// 動画拡張子
		/// </summary>
		SettingsCollection<string> VideoExtensions {
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
		/// 一覧表示ズームレベル
		/// </summary>
		SettingsItem<int> ZoomLevel {
			get;
		}


		/// <summary>
		/// 表示モード
		/// </summary>
		SettingsItem<DisplayMode> DisplayMode {
			get;
		}

		/// <summary>
		/// 外部ツール
		/// </summary>
		SettingsCollection<ExternalToolParams> ExternalTools {
			get;
		}

		/// <summary>
		/// デフォルト設定ロード
		/// </summary>
		void LoadDefault();
	}
}