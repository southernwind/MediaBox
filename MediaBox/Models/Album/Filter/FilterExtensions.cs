using System.Collections.Generic;
using System.Linq;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;


namespace SandBeige.MediaBox.Models.Album.Filter {
	public static class FilterExtensions {
		/// <summary>
		/// フィルターマネージャーで選択したフィルターを引数に渡されたクエリに適用して返却する。
		/// </summary>
		/// <param name="query">絞り込みクエリを適用するクエリ</param>
		/// <param name="filter">適用するフィルター</param>
		/// <returns>フィルター適用後クエリ</returns>
		public static IEnumerable<MediaFile> Where(this IQueryable<MediaFile> query, IFilterDescriptionManager filter) {
			return filter.SetFilterConditions(query);
		}

		/// <summary>
		/// フィルターマネージャーで選択したフィルターを引数に渡されたシーケンスに適用して返却する。
		/// </summary>
		/// <param name="files">絞り込みを適用するシーケンス</param>
		/// <param name="filter">適用するフィルター</param>
		/// <returns>フィルター適用後シーケンス</returns>
		public static IEnumerable<IMediaFileModel> Where(this IEnumerable<IMediaFileModel> files, IFilterDescriptionManager filter) {
			return filter.SetFilterConditions(files);
		}
	}
}
