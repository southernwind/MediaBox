using System.Collections.Generic;

namespace SandBeige.MediaBox.Library.Extensions {
	/// <summary>
	/// <see cref="Dictionary{TKey, TValue}"/>の拡張メソッドクラス
	/// </summary>
	public static class DictionaryEx {
		/// <summary>
		/// 取得、できなければデフォルト値
		/// </summary>
		/// <param name="collection">取得元コレクション</param>
		/// <param name="key">取得キー</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>取得値、もしくはデフォルト値</returns>
		public static int? GetOrDefault(this Dictionary<string, string> collection, string key, int? defaultValue) {
			if (collection != null && collection.TryGetValue(key, out var value) && int.TryParse(value, out var result)) {
				return result;
			}
			return defaultValue;
		}

		/// <summary>
		/// 取得、できなければデフォルト値
		/// </summary>
		/// <param name="collection">取得元コレクション</param>
		/// <param name="key">取得キー</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>取得値、もしくはデフォルト値</returns>
		public static double? GetOrDefault(this Dictionary<string, string> collection, string key, double? defaultValue) {
			if (collection != null && collection.TryGetValue(key, out var value) && double.TryParse(value, out var result)) {
				return result;
			}
			return defaultValue;
		}
	}
}
