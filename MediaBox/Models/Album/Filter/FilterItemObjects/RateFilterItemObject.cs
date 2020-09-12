using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Enum;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects {
	/// <summary>
	/// 評価フィルターアイテムオブジェクト
	/// </summary>
	public class RateFilterItemObject : IFilterItemObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				var com = new Dictionary<SearchTypeComparison, string> {
					{SearchTypeComparison.GreaterThan, "を超える"},
					{SearchTypeComparison.GreaterThanOrEqual, "以上"},
					{SearchTypeComparison.Equal, "と等しい"},
					{SearchTypeComparison.LessThanOrEqual, "以下"},
					{SearchTypeComparison.LessThan, "未満"}
				}[this.SearchType];
				return $"評価が{this.Rate}{com}";
			}
		}

		/// <summary>
		/// 評価
		/// </summary>
		public int Rate {
			get;
			set;
		}

		/// <summary>
		/// 検索タイプ
		/// </summary>
		public SearchTypeComparison SearchType {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public RateFilterItemObject() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="rate">評価</param>
		/// <param name="searchType">検索タイプ</param>
		public RateFilterItemObject(int rate, SearchTypeComparison searchType) {
			if (!Enum.IsDefined(typeof(SearchTypeComparison), searchType)) {
				throw new ArgumentException();
			}
			this.Rate = rate;
			this.SearchType = searchType;
		}
	}
}
