using System;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// 解像度フィルタークリエイター
	/// </summary>
	public class ResolutionFilterItemCreator : IFilterItemCreator<ResolutionFilterItemObject> {
		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create(ResolutionFilterItemObject filterItemObject) {
			var op = SearchTypeConverters.SearchTypeToFunc<double?>(filterItemObject.SearchType);
			if (filterItemObject.Width is { } w) {
				return new FilterItem(
					x => op(x.Width, w),
					x => op(x.Resolution?.Width, w),
					false);
			} else if (filterItemObject.Height is { } h) {
				return new FilterItem(
					x => op(x.Height, h),
					x => op(x.Resolution?.Height, h),
					false);
			} else if (filterItemObject.Resolution is { } r) {
				return new FilterItem(
					x => op(x.Width * x.Height, r.Area),
					x => op(x.Resolution?.Area, r.Area),
					false);
			}
			throw new InvalidOperationException();
		}
	}
}
