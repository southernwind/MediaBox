
using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.History.Creator;
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
		IReactiveProperty<IAlbumModel> CurrentAlbum {
			get;
		}

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="album"></param>
		void SetAlbumToCurrent(IAlbumModel album);

		/// <summary>
		/// 引数のアルバムをカレントする
		/// </summary>
		/// <param name="albumCreator"></param>
		void SetAlbumToCurrent(IAlbumCreator albumCreator);

		/// <summary>
		/// フォルダアルバムをカレントにする
		/// </summary>
		void SetFolderAlbumToCurrent();

		/// <summary>
		/// データベース検索アルバムをカレントにする
		/// </summary>
		/// <param name="albumTitle">アルバムタイトル</param>
		/// <param name="tagName">タグ名</param>
		void SetDatabaseAlbumToCurrent(string albumTitle, string tagName);

		/// <summary>
		/// 場所検索アルバムをカレントにする
		/// </summary>
		/// <param name="albumTitle">アルバムタイトル</param>
		/// <param name="positions">場所情報</param>
		void PositionSearchAlbumToCurrent(string albumTitle, Address positions);

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		void DeleteAlbum(IAlbumModel album);
	}
}
