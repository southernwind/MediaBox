using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Library.Collection {

	/// <summary>
	/// タイトルと値のペア
	/// </summary>
	public class TitleValuePair {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="value">値</param>
		public TitleValuePair(string title, string value) {
			this.Title = title;
			this.Value = value;
		}

		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// 値
		/// </summary>
		public string Value {
			get;
			set;
		}
	}

	public static class TitleValuePairEx {
		public static IEnumerable<TitleValuePair> ToTitleValuePair(this Dictionary<string, string> source) {
			return source.Select(x => new TitleValuePair(x.Key, x.Value));
		}
	}
}
