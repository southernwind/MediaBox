using System;
using System.Linq;

using SandBeige.MediaBox.Composition.Enum;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// タグフィルタークリエイター
	/// </summary>
	public class TagFilterItemCreator : IFilterItemCreator {
		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get {
				return $"{this.TagName}をタグに{(this.SearchType == SearchType.Include ? "含む" : "含まない")}";
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
		public SearchType SearchType {
			get;
			set;
		}

		[Obsolete("for serialize")]
		public TagFilterItemCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="tagName">タグ名</param>
		/// <param name="searchType">検索タイプ</param>
		public TagFilterItemCreator(string tagName, SearchType searchType) {
			this.TagName = tagName;
			this.SearchType = searchType;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create() {
			return new FilterItem(
				x => x.MediaFileTags.Select(mft => mft.Tag.TagName).Contains(this.TagName) == (this.SearchType == SearchType.Include),
				x => x.Tags.Contains(this.TagName) == (this.SearchType == SearchType.Include));
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.DisplayName}>";
		}
	}
}
