﻿using System;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	public class RateFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"評価が{this.Rate}以上";
			}
		}

		/// <summary>
		/// 評価
		/// </summary>
		public int Rate {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public RateFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public RateFilterItemCreator(int rate) {
			this.Rate = rate;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(x => x.Rate >= this.Rate);
		}
	}
}