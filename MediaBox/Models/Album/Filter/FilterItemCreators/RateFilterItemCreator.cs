using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Enum;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// 評価フィルタークリエイター
	/// </summary>
	public class RateFilterItemCreator : IFilterItemCreator {
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
		public RateFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="rate">評価</param>
		/// <param name="searchType">検索タイプ</param>
		public RateFilterItemCreator(int rate, SearchTypeComparison searchType) {
			if (!Enum.IsDefined(typeof(SearchTypeComparison), searchType)) {
				throw new ArgumentException();
			}
			this.Rate = rate;
			this.SearchType = searchType;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			var op = SearchTypeConverters.SearchTypeToFunc<int>(this.SearchType);
			return new FilterItem(
				x => op(x.Rate, this.Rate),
				x => op(x.Rate, this.Rate),
				false);
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
