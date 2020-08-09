using System;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Map;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album {
	public interface IAlbumSelector : IDisposable {
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
		IAlbumModel Album {
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
		void SetPositionSearchAlbumToCurrent(IAddress positions);

		/// <summary>
		/// アルバム削除
		/// </summary>
		/// <param name="album">削除対象アルバム</param>
		void DeleteAlbum(IAlbumObject album);

		/// <summary>
		/// 名称設定
		/// </summary>
		/// <param name="name">一意になる名称 フィルターとソート順の保存、復元に使用する。</param>
		void SetName(string name);
	}
}
