
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// タグフィルタークリエイター
	/// </summary>
	public class TagFilterItemCreator : IFilterItemCreator<TagFilterItemObject> {
		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create(TagFilterItemObject filterItemObject) {
			return new FilterItem(
				x => (x.Tags != null && x.Tags.Contains(filterItemObject.TagName)) == (filterItemObject.SearchType == SearchTypeInclude.Include),
				x => (x.Tags.Contains(filterItemObject.TagName)) == (filterItemObject.SearchType == SearchTypeInclude.Include),
				false);
		}
	}
}
