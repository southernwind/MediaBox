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
}
