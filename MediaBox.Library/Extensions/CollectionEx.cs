using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SandBeige.MediaBox.Library.Extensions {
	/// <summary>
	/// コレクション拡張メソッドクラス
	/// </summary>
	public static class CollectionEx {
		/// <summary>
		/// 複数追加
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">追加先コレクション</param>
		/// <param name="items">追加するコレクション</param>
		public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> items) {
			foreach (var item in items.ToArray()) {
				source.Add(item);
			}
		}

		/// <summary>
		/// 複数削除
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">削除先コレクション</param>
		/// <param name="items">削除するコレクション</param>
		public static void RemoveRange<T>(this ICollection<T> source, IEnumerable<T> items) {
			foreach (var item in items.ToArray()) {
				source.Remove(item);
			}
		}

		/// <summary>
		/// 複数追加
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">追加先コレクション</param>
		/// <param name="items">追加するコレクション</param>
		public static void AddRange<T>(this ICollection<T> source, params T[] items) {
			source.AddRange(items as IEnumerable<T>);
		}

		/// <summary>
		/// 複数削除
		/// </summary>
		/// <typeparam name="T">要素型</typeparam>
		/// <param name="source">削除先コレクション</param>
		/// <param name="items">削除するコレクション</param>
		public static void RemoveRange<T>(this ICollection<T> source, params T[] items) {
			source.RemoveRange(items as IEnumerable<T>);
		}

		/// <summary>
		/// コレクションロック
		/// </summary>
		/// <typeparam name="T">コレクション型</typeparam>
		/// <typeparam name="T2">戻り値型</typeparam>
		/// <param name="collection">コレクション</param>
		/// <param name="func">コレクションに対する操作</param>
		/// <returns>操作結果</returns>
		public static T2 Lock<T, T2>(this T collection, Func<T, T2> func) where T : ICollection {
			lock (collection.SyncRoot) {
				return func(collection);
			}
		}

		/// <summary>
		/// コレクションロック
		/// </summary>
		/// <typeparam name="T">コレクション型</typeparam>
		/// <param name="collection">コレクション</param>
		/// <param name="func">コレクションに対する操作</param>
		public static void Lock<T>(this T collection, Action<T> func) where T : ICollection {
			lock (collection.SyncRoot) {
				func(collection);
			}
		}
	}
}