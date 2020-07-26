using SandBeige.MediaBox.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Models.Map;

namespace SandBeige.MediaBox.Models.Album {
	public interface IAlbumSelector {
		/// <summary>
		/// フィルター
		/// </summary>
		IFilterSetter FilterSetter {
			get;
		}

		/// <summary>
		/// ソート
		/// </summary>
		ISortSetter SortSetter {
			get;
		}

		/// <summary>
		/// カレントアルバム
		/// </summary>
		AlbumModel Album {
			get;
		}

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="album"></param>
		void SetAlbumToCurrent(IAlbumObject album);

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		void SetFolderAlbumToCurrent(string path);

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="tagName">タグ名</param>
		void SetDatabaseAlbumToCurrent(string tagName);

		/// <summary>
		/// ワード検索アルバムをカレントにする
		/// </summary>
		/// <param name="word">検索ワード</param>
		void SetWordSearchAlbumToCurrent(string word);

		/// <summary>
		/// 場所検索アルバムをカレントにする
		/// </summary>
		/// <param name="positions">場所情報</param>
		void SetPositionSearchAlbumToCurrent(Address positions);

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		void DeleteAlbum(IAlbumObject album);
	}
}
