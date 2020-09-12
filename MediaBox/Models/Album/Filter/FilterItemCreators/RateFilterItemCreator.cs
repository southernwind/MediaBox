
using SandBeige.MediaBox.Composition.Enum;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// 評価フィルタークリエイター
	/// </summary>
	public class RateFilterItemCreator : IFilterItemCreator<RateFilterItemObject> {
		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create(RateFilterItemObject filterItemObject) {
			var op = SearchTypeConverters.SearchTypeToFunc<int>(filterItemObject.SearchType);
			return new FilterItem(
				x => op(x.Rate, filterItemObject.Rate),
				x => op(x.Rate, filterItemObject.Rate),
				false);
		}
	}
}
