using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Library.Collection {

	/// <summary>
	/// タイトルと値のペア
	/// </summary>
	public class TitleValuePair<T> {
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="value">値</param>
		public TitleValuePair(string title, T value) {
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
		public T Value {
			get;
			set;
		}
	}

	public static class TitleValuePairEx {
		public static IEnumerable<TitleValuePair<T>> ToTitleValuePair<T>(this Dictionary<string, T> source) {
			return source.Select(x => new TitleValuePair<T>(x.Key, x.Value));
		}
	}
}
