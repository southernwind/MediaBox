namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter {
	/// <summary>
	/// フィルタークリエイターインターフェイス
	/// </summary>
	public interface IFilterItemCreator<in T> where T : IFilterItemObject {

		/// <summary>
		/// フィルター条件の作成
		/// </summary>
		/// <returns>作成された条件</returns>
		IFilterItem Create(T filterItemObject);
	}
}
