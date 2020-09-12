using System.IO;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// ファイル存在フィルタークリエイター
	/// </summary>
	public class ExistsFilterItemCreator : IFilterItemCreator<ExistsFilterItemObject> {
		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create(ExistsFilterItemObject filterItemObject) {
			return new FilterItem(
				x => File.Exists(x.FilePath) == filterItemObject.Exists,
				x => x.Exists == filterItemObject.Exists,
				false);
		}
	}
}