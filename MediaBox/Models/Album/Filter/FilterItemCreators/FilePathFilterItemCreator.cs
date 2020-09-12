
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;


namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// ファイルパスフィルタークリエイター
	/// </summary>
	public class FilePathFilterItemCreator : IFilterItemCreator<FilePathFilterItemObject> {
		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		public IFilterItem Create(FilePathFilterItemObject filterItemObject) {
			return new FilterItem(
				x => x.FilePath.Contains(filterItemObject.Text) == (filterItemObject.SearchType == SearchTypeInclude.Include),
				x => x.FilePath.Contains(filterItemObject.Text) == (filterItemObject.SearchType == SearchTypeInclude.Include),
				true);
		}
	}
}
