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

	/// <summary>
	/// タイトルと値のペアの拡張メソッドクラス
	/// </summary>
	public static class TitleValuePairEx {
		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/>→<see cref="IEnumerable{TitleValuePair{T}}"/>
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">ソースとなる<see cref="Dictionary{TKey, TValue}"/></param>
		/// <returns>作成された<see cref="IEnumerable{TitleValuePair{T}}"/></returns>
		public static IEnumerable<TitleValuePair<T>> ToTitleValuePair<T>(this Dictionary<string, T> source) {
			return source.Select(x => new TitleValuePair<T>(x.Key, x.Value));
		}
	}
}
