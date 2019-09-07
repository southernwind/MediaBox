using System;
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Objects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// 解像度フィルタークリエイター
	/// </summary>
	public class ResolutionFilterItemCreator : IFilterItemCreator {
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
				if (this.Width != null) {
					return $"幅が{this.Width}{com}";
				}
				if (this.Height != null) {
					return $"高さが{this.Height}{com}";
				}
				if (this.Resolution != null) {
					return $"解像度が{this.Resolution}{com}";
				}
				throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// 幅
		/// </summary>
		public int? Width {
			get;
			set;
		}

		/// <summary>
		/// 高さ
		/// </summary>
		public int? Height {
			get;
			set;
		}

		/// <summary>
		/// 解像度
		/// </summary>
		public ComparableSize? Resolution {
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
		public ResolutionFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="resolution">解像度</param>
		/// <param name="searchType">検索タイプ</param>
		public ResolutionFilterItemCreator(ComparableSize resolution, SearchTypeComparison searchType) {
			if (!Enum.IsDefined(typeof(SearchTypeComparison), searchType)) {
				throw new ArgumentException();
			}
			this.Resolution = resolution;
			this.SearchType = searchType;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="width">幅</param>
		/// <param name="height">高さ</param>
		/// <param name="searchType">検索タイプ</param>
		public ResolutionFilterItemCreator(int? width, int? height, SearchTypeComparison searchType) {
			if (!(width == null ^ height == null) || !Enum.IsDefined(typeof(SearchTypeComparison), searchType)) {
				throw new ArgumentException();
			}
			this.Width = width;
			this.Height = height;
			this.SearchType = searchType;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			var op = SearchTypeConverters.SearchTypeToFunc<double?>(this.SearchType);
			if (this.Width is { } w) {
				return new FilterItem(
					x => op(x.Width, w),
					x => op(x.Resolution?.Width, w));
			} else if (this.Height is { } h) {
				return new FilterItem(
					x => op(x.Height, h),
					x => op(x.Resolution?.Height, h));
			} else if (this.Resolution is { } r) {
				return new FilterItem(
					x => op(x.Width * x.Height, r.Area),
					x => op(x.Resolution?.Area, r.Area));
			}
			throw new InvalidOperationException();
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
