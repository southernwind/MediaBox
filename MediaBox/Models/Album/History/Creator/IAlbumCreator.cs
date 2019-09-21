using SandBeige.MediaBox.Composition.Interfaces;

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
		/// <param name="selector">作成するアルバムを保有するセレクター</param>
		/// <returns>作成されたアルバム</returns>
		IAlbumModel Create(IAlbumSelector selector);
	}
}
