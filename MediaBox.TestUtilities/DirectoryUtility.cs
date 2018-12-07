using System.IO;
using System.Linq;
using System.Threading;

namespace MediaBox.TestUtilities {
	public static class DirectoryUtility {

		/// <summary>
		/// ディレクトリ再帰削除
		/// </summary>
		/// <param name="path">ディレクトリパス</param>
		public static void DirectoryDelete(string path) {
			if (!Directory.Exists(path)) {
				return;
			}
			foreach (var file in Directory.GetFiles(path)) {
				File.Delete(file);
			}
			foreach (var directory in Directory.GetDirectories(path)) {
				DirectoryDelete(directory);
			}
			for (var i = 0; i < 5 && Directory.EnumerateFileSystemEntries(path).Any(); i++) {
				Thread.Sleep(100);
			}
			Directory.Delete(path);

			Thread.Sleep(100);
		}

	}
}
