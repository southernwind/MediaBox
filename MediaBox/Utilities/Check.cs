using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SandBeige.MediaBox.Composition.Logging;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Utilities {
	internal static class Check {
		/// <summary>
		/// 指定したファイルパスのファイルが管理対象の拡張子を持っているかどうかを調べる
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <returns>管理対象か否か</returns>
		public static bool IsTargetExtension(this string path){
			return Get.Instance<ISettings>().GeneralSettings.TargetExtensions.Value.Contains(Path.GetExtension(path).ToLower());
		}
	}
}
