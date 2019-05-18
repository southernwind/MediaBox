using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;

namespace SandBeige.MediaBox.Models.Album.History.Creator {
	/// <summary>
	/// アルバム作成インターフェイス
	/// </summary>
	public interface IAlbumCreator {
		/// <summary>
		/// タイトル
		/// </summary>
		string Title {
			get;
		}

		/// <summary>
		/// アルバムの作成
		/// </summary>
		/// <param name="filter">アルバムに適用するフィルター</param>
		/// <param name="sort">アルバムに適用するソート</param>
		/// <returns>作成されたアルバム</returns>
		IAlbumModel Create(IFilterSetter filter, ISortSetter sort);
	}
}
