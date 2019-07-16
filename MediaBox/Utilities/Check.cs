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
		/// <returns>管理対象か否か</returns>
		public static bool IsTargetExtension(this string path) {
			var gs = Get.Instance<ISettings>().GeneralSettings;
			return gs.ImageExtensions.Union(gs.VideoExtensions)
				.Contains(Path.GetExtension(path)?.ToLower());
		}

		/// <summary>
		/// 指定したファイルパスのファイルが動画拡張子を持っているかどうかを調べる
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <returns>動画ファイルか否か</returns>
		public static bool IsVideoExtension(this string path) {
			return
				Get.Instance<ISettings>()
					.GeneralSettings
					.VideoExtensions
					.Contains(Path.GetExtension(path)?.ToLower());
		}
	}
}
