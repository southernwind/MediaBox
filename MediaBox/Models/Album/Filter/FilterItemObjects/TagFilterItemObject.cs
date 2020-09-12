using System;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects {
	/// <summary>
	/// タグフィルターアイテムオブジェクト
	/// </summary>
	public class TagFilterItemObject : IFilterItemObject {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"{this.TagName}をタグに{(this.SearchType == SearchTypeInclude.Include ? "含む" : "含まない")}";
			}
		}

		/// <summary>
		/// タグ名
		/// </summary>
		public string TagName {
			get;
			set;
		}

		/// <summary>
		/// 検索タイプ
		/// </summary>
		public SearchTypeInclude SearchType {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public TagFilterItemObject() {
			this.TagName = null!;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tagName">タグ名</param>
		/// <param name="searchType">検索タイプ</param>
		public TagFilterItemObject(string tagName, SearchTypeInclude searchType) {
			if (tagName == null || !Enum.IsDefined(typeof(SearchTypeInclude), searchType)) {
				throw new ArgumentException();
			}
			this.TagName = tagName;
			this.SearchType = searchType;
		}
	}
}
