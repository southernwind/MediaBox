using System.Linq;

using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Library.Extensions {
	/// <summary>
	/// <see cref="Attributes{T}"/>の拡張メソッドクラス
	/// </summary>
	public static class AttributesEx {
		/// <summary>
		/// 取得、できなければデフォルト値
		/// </summary>
		/// <param name="attributes">取得元属性リスト</param>
		/// <param name="title">タイトル</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>取得値、もしくはデフォルト値</returns>
		public static int? GetOrDefault(this Attributes<string> attributes, string title, int? defaultValue) {
			var value = attributes.FirstOrDefault(x => x.Title == title).Value;
			if (int.TryParse(value, out var result)) {
				return result;
			}
			return defaultValue;
		}

		/// <summary>
		/// 取得、できなければデフォルト値
		/// </summary>
		/// <param name="attributes">取得元属性リスト</param>
		/// <param name="title">タイトル</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>取得値、もしくはデフォルト値</returns>
		public static double? GetOrDefault(this Attributes<string> attributes, string title, double? defaultValue) {
			var value = attributes.FirstOrDefault(x => x.Title == title).Value;
			if (double.TryParse(value, out var result)) {
				return result;
			}
			return defaultValue;
		}
	}
}
