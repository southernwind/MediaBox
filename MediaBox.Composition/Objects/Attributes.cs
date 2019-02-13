using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Composition.Objects {
	/// <summary>
	/// 属性リスト
	/// </summary>
	/// <typeparam name="T">属性値の型</typeparam>
	public class Attributes<T> : IEnumerable<TitleValuePair<T>> {
		private readonly List<TitleValuePair<T>> _list = new List<TitleValuePair<T>>();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Attributes() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">生成元<see cref="Dictionary{TKey, TValue}"/></param>
		public Attributes(Dictionary<string, T> source) {
			foreach (var item in source) {
				this.Add(item.Key, item.Value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">生成元<see cref="IEnumerable{T}"/></param>
		public Attributes(IEnumerable<TitleValuePair<T>> source) {
			foreach (var item in source) {
				this.Add(item);
			}
		}

		/// <summary>
		/// 要素追加
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="value">値</param>
		public void Add(string title, T value) {
			this.Add(new TitleValuePair<T>(title, value));
		}

		/// <summary>
		/// 要素追加
		/// </summary>
		/// <param name="item">タイトルと値のペア</param>
		public void Add(TitleValuePair<T> item) {
			this._list.Add(item);
		}

		public IEnumerator<TitleValuePair<T>> GetEnumerator() {
			return this._list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return this.GetEnumerator();
		}
	}

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
	public static class AttributesEx {
		/// <summary>
		/// <see cref="Dictionary{TKey, TValue}"/>→<see cref="Attributes{T}"/>
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">ソースとなる<see cref="Dictionary{TKey, TValue}"/></param>
		/// <returns>作成された<see cref="Attributes{T}"/></returns>
		public static Attributes<T> ToAttributes<T>(this Dictionary<string, T> source) {
			return new Attributes<T>(source);
		}

		/// <summary>
		/// <see cref="IEnumerable{T}"/>→<see cref="Attributes{T}"/>
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">ソースとなる変換前リスト</param>
		/// <returns>作成された<see cref="Attributes{T}"/></returns>
		public static Attributes<T> ToAttributes<T>(this IEnumerable<TitleValuePair<T>> source) {
			return new Attributes<T>(source);
		}



		/// <summary>
		/// <see cref="IEnumerable{T}"/>→<see cref="Attributes{T}"/>
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <typeparam name="TSource">変換前リストの要素の型</typeparam>
		/// <param name="source">ソースとなる変換前リスト</param>
		/// <param name="titleSelector">タイトル生成関数</param>
		/// <param name="valueSelector">値生成関数</param>
		/// <returns>作成された<see cref="Attributes{T}"/></returns>
		public static Attributes<T> ToAttributes<TSource, T>(this IEnumerable<TSource> source, Func<TSource, string> titleSelector, Func<TSource, T> valueSelector) {
			return new Attributes<T>(source.Select(x => new TitleValuePair<T>(titleSelector(x), valueSelector(x))));
		}
	}
}
