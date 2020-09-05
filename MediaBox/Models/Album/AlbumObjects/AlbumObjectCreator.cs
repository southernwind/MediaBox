using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;

namespace SandBeige.MediaBox.Models.Album.AlbumObjects {
	public class AlbumObjectCreator : ModelBase, IAlbumObjectCreator {

		/// <summary>
		/// フォルダアルバムを作成する。
		/// </summary>
		public IAlbumObject CreateFolderAlbum(string path) {
			var fao = new FolderAlbumObject(path);
			return fao;
		}

		/// <summary>
		/// データベース検索アルバムを作成する。
		/// </summary>
		/// <param name="tagName">タグ名</param>
		public IAlbumObject CreateDatabaseAlbum(string tagName) {
			var ldao = new LookupDatabaseAlbumObject {
				TagName = tagName
			};
			return ldao;
		}

		/// <summary>
		/// ワード検索アルバムを作成する。
		/// </summary>
		/// <param name="word">検索ワード</param>
		public IAlbumObject CreateWordSearchAlbum(string word) {
			var ldao = new LookupDatabaseAlbumObject {
				Word = word
			};
			return ldao;
		}

		/// <summary>
		/// 場所検索アルバムを作成する。
		/// </summary>
		/// <param name="address">場所情報</param>
		public IAlbumObject CreatePositionSearchAlbum(IAddress address) {
			var ldao = new LookupDatabaseAlbumObject {
				Address = address
			};
			return ldao;
		}
	}
}
