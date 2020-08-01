using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;


namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object {
	public interface IAlbumObjectCreator {

		/// <summary>
		/// フォルダアルバムを作成する。
		/// </summary>
		IAlbumObject CreateFolderAlbum(string path);

		/// <summary>
		/// データベース検索アルバムを作成する。
		/// </summary>
		/// <param name="tagName">タグ名</param>
		IAlbumObject CreateDatabaseAlbum(string tagName);

		/// <summary>
		/// ワード検索アルバムを作成する。
		/// </summary>
		/// <param name="word">検索ワード</param>
		IAlbumObject CreateWordSearchAlbum(string word);

		/// <summary>
		/// 場所検索アルバムを作成する。
		/// </summary>
		/// <param name="address">場所情報</param>
		IAlbumObject CreatePositionSearchAlbum(IAddress address);
	}

}
