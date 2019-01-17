using System.IO;

using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Utilities {
	internal static class Check {
		/// <summary>
		/// 指定したファイルパスのファイルが管理対象の拡張子を持っているかどうかを調べる
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <returns>管理対象か否か</returns>
		public static bool IsTargetExtension(this string path) {
			return
				Get.Instance<ISettings>()
					.GeneralSettings
					.TargetExtensions
					.Contains(Path.GetExtension(path)?.ToLower());
		}
	}
}
