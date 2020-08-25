using System.IO;
using System.Linq;

using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Utilities {
	/// <summary>
	/// チェッククラス
	/// </summary>
	internal static class Check {
		/// <summary>
		/// 指定したファイルパスのファイルが管理対象の拡張子を持っているかどうかを調べる
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="settings">設定オブジェクト</param>
		/// <returns>管理対象か否か</returns>
		public static bool IsTargetExtension(this string path, ISettings settings) {
			return settings
				.GeneralSettings
				.ImageExtensions
				.Union(settings.GeneralSettings.VideoExtensions)
				.Contains(Path.GetExtension(path)?.ToLower());
		}

		/// <summary>
		/// 指定したファイルパスのファイルが動画拡張子を持っているかどうかを調べる
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="settings">設定オブジェクト</param>
		/// <returns>動画ファイルか否か</returns>
		public static bool IsVideoExtension(this string path, ISettings settings) {
			return settings
					.GeneralSettings
					.VideoExtensions
					.Contains(Path.GetExtension(path)?.ToLower());
		}
	}
}
