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
		private static readonly ISettings _settings;
		private static readonly ILogging _logging;
		
		static Check() {
			_settings = Get.Instance<ISettings>();
			_logging = Get.Instance<ILogging>();
		}
		
		/// <summary>
		/// 指定したファイルパスのファイルが管理対象の拡張子を持っているかどうかを調べる
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <returns>管理対象か否か</returns>
		public static bool IsTargetExtension(this string path){
			return _settings.GeneralSettings.TargetExtensions.Value.Contains(Path.GetExtension(path).ToLower());
		}
	}
}
