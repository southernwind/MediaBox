using System.IO;

namespace SandBeige.MediaBox.TestUtilities {
	public static class DirectoryUtility {

		/// <summary>
		/// ディレクトリ再帰削除
		/// </summary>
		/// <param name="path">ディレクトリパス</param>
		public static void AllFileDelete(string path) {
			if (!Directory.Exists(path)) {
				return;
			}
			foreach (var file in Directory.GetFiles(path)) {
				File.Delete(file);
			}
			foreach (var directory in Directory.GetDirectories(path)) {
				AllFileDelete(directory);
			}
		}

	}
}
