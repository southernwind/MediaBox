namespace SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators {
	/// <summary>
	/// フィルタークリエイターインターフェイス
	/// </summary>
	public interface IFilterItemCreator {

		/// <summary>
		/// 表示名
		/// </summary>
		string DisplayName {
			get;
		}

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		IFilterItem Create();
	}
}
