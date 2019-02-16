using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SandBeige.MediaBox.Library.IO {
	/// <summary>
	/// ディレクトリ拡張
	/// </summary>
	public static class DirectoryEx {
		/// <summary>
		/// 全ファイル抽出
		/// </summary>
		/// <param name="directoryPath">対象フォルダ</param>
		/// <returns>ファイルリスト</returns>
		public static IEnumerable<string> EnumerateFiles(string directoryPath) {
			try {
				return
					Directory
						.EnumerateFiles(directoryPath)
						.Concat(
							Directory
								.EnumerateDirectories(directoryPath)
								.SelectMany(EnumerateFiles));
			} catch (Exception ex) when (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException) {
				return Enumerable.Empty<string>();
			}
		}
	}
}
