
using System;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;
using SandBeige.MediaBox.Composition.Settings;
using SandBeige.MediaBox.Composition.Settings.Objects;

namespace SandBeige.MediaBox.Models.Settings {
	public class GeneralSettings : SettingsBase, IGeneralSettings {

		/// <summary>
		/// 画像拡張子
		/// </summary>

		public SettingsCollection<string> ImageExtensions {
			get;
		} = new SettingsCollection<string>(".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff", ".tif");


		/// <summary>
		/// 動画拡張子
		/// </summary>
		public SettingsCollection<string> VideoExtensions {
			get;
		} = new SettingsCollection<string>(
			".avi",
			".mp4",
			".m4a",
			".mov",
			".qt",
			".m2ts",
			".ts",
			".mpeg",
			".mpg",
			".mkv",
			".wmv",
			".asf",
			".flv",
			".f4v",
			".wmv",
			".webm",
			".ogm");

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
		/// 一覧表示ズームレベル
		/// </summary>
		public SettingsItem<int> ZoomLevel {
			get;
			set;
		} = new SettingsItem<int>(Controls.Converters.ZoomLevel.DefaultLevel);

		/// <summary>
		/// 外部ツール
		/// </summary>
		public SettingsCollection<ExternalToolParams> ExternalTools {
			get;
		} = new SettingsCollection<ExternalToolParams>(Array.Empty<ExternalToolParams>());

		/// <summary>
		/// 有効な表示列
		/// </summary>
		public SettingsCollection<AvailableColumns> EnabledColumns {
			get;
		} = new SettingsCollection<AvailableColumns>(
			AvailableColumns.Thumbnail,
			AvailableColumns.FileName,
			AvailableColumns.ModifiedTime);
	}
}
