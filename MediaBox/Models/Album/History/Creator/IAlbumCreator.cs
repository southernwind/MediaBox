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
		/// <returns>作成されたアルバム</returns>
		IAlbumModel Create();
	}
}
