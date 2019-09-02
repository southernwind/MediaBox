using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SandBeige.MediaBox.Composition.Enum {
	/// <summary>
	/// 検索タイプ(含む/含まない)
	/// </summary>
	public enum SearchTypeInclude {
		/// <summary>
		/// 含むものを検索
		/// </summary>
		Include,
		/// <summary>
		/// 含まないものを検索
		/// </summary>
		Exclude
	}

	/// <summary>
	/// 検索タイプ(比較)
	/// </summary>
	public enum SearchTypeComparison {
		/// <summary>
		/// 超
		/// </summary>
		GreaterThan,
		/// <summary>
		/// 以上
		/// </summary>
		GreaterThanOrEqual,
		/// <summary>
		/// 同値
		/// </summary>
		Equal,
		/// <summary>
		/// 以下
		/// </summary>
		LessThanOrEqual,
		/// <summary>
		/// 未満
		/// </summary>
		LessThan
	}

	public static class SearchTypeConverters {
		/// <summary>
		/// 検索タイプ毎に適切な比較メソッドを返却する。
		/// </summary>
		/// <typeparam name="T">比較する型</typeparam>
		/// <param name="searchType">検索タイプ</param>
		/// <returns>比較用メソッド</returns>
		public static Func<T, T, bool> SearchTypeToFunc<T>(SearchTypeComparison searchType) {
			var op = new Dictionary<SearchTypeComparison, Func<Expression, Expression, BinaryExpression>> {
				{SearchTypeComparison.GreaterThan, Expression.GreaterThan},
				{SearchTypeComparison.GreaterThanOrEqual,Expression.GreaterThanOrEqual},
				{SearchTypeComparison.Equal, Expression.Equal},
				{SearchTypeComparison.LessThanOrEqual,Expression.LessThanOrEqual},
				{SearchTypeComparison.LessThan, Expression.LessThan}
			}.First(x => x.Key == searchType).Value;

			var p1 = Expression.Parameter(typeof(T));
			var p2 = Expression.Parameter(typeof(T));
			var func = Expression.Lambda<Func<T, T, bool>>(op(p1, p2), p1, p2);

			return func.Compile();
		}
	}
}
