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
		/// <param name="includeSubdirectories">サブディレクトリを含むか否か</param>
		/// <returns>ファイルリスト</returns>
		public static IEnumerable<string> EnumerateFiles(string directoryPath, bool includeSubdirectories) {
			try {
				var result = Directory.EnumerateFiles(directoryPath);

				if (includeSubdirectories) {
					result = result.Concat(
						Directory
							.EnumerateDirectories(directoryPath)
							.SelectMany(x => EnumerateFiles(x, true)));
				}
				return result;
			} catch (Exception ex) when (ex is UnauthorizedAccessException || ex is DirectoryNotFoundException) {
				return Enumerable.Empty<string>();
			}
		}
	}
}
