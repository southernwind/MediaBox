using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Library.Video {

	/// <summary>
	/// 動画メタデータ
	/// </summary>
	public class Metadata {
		internal Dictionary<string, string> Formats {
			get;
			set;
		}

		internal IEnumerable<Dictionary<string, string>> Streams {
			get;
			set;
		}

		/// <summary>
		/// 動画の長さ
		/// </summary>
		public double? Duration {
			get {
				return GetOrDefault(this.Formats, "duration", (double?)null);
			}
		}

		/// <summary>
		/// 回転
		/// </summary>
		public int? Rotation {
			get {
				return GetOrDefault(
					this.Streams.SingleOrDefault(x => x.Any(kv => kv.Key == "codec_type" && kv.Value == "video")),
					"rotation",
					null);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal Metadata() {
		}


		/// <summary>
		/// 取得、できなければデフォルト値
		/// </summary>
		/// <param name="collection">取得元コレクション</param>
		/// <param name="key">取得キー</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>取得値、もしくはデフォルト値</returns>
		private static int? GetOrDefault(Dictionary<string, string> collection, string key, int? defaultValue) {
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
		private static double? GetOrDefault(Dictionary<string, string> collection, string key, double? defaultValue) {
			if (collection != null && collection.TryGetValue(key, out var value) && double.TryParse(value, out var result)) {
				return result;
			}
			return defaultValue;
		}
	}
}
