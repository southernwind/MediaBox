using System.Collections.Generic;

using MetadataExtractor;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Image.Formats {
	/// <summary>
	/// フォーマット用ユーティリティ
	/// </summary>
	internal static class Utility {
		/// <summary>
		/// ディレクトリリストをプロパティに変換
		/// </summary>
		/// <param name="directories">ソースディレクトリリスト</param>
		/// <returns>変換後プロパティ</returns>
		public static Attributes<Attributes<string>> ToProperties(this IReadOnlyList<Directory> directories) {
			return
				directories
					.ToAttributes(
						d => d.Name,
						d =>
							d.Tags
								.ToAttributes(
									x => x.Name,
									x => x.Description
								)
					);
		}
	}
}
