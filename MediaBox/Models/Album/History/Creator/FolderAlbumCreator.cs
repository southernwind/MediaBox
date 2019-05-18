using System;

using SandBeige.MediaBox.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Sort;
using SandBeige.MediaBox.Utilities;

namespace SandBeige.MediaBox.Models.Album.History.Creator {
	/// <summary>
	/// フォルダアルバム作成
	/// </summary>
	public class FolderAlbumCreator : IAlbumCreator {

		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// フォルダパス
		/// </summary>
		public string FolderPath {
			get;
			set;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		[Obsolete("for serialize")]
		public FolderAlbumCreator() {
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="title">タイトル</param>
		/// <param name="folderPath">フォルダパス</param>
		public FolderAlbumCreator(string title, string folderPath) {
			this.Title = title;
			this.FolderPath = folderPath;
		}

		/// <summary>
		/// アルバムの作成
		/// </summary>
		/// <param name="filter">アルバムに適用するフィルター</param>
		/// <param name="sort">アルバムに適用するソート</param>
		/// <returns>作成されたアルバム</returns>
		public IAlbumModel Create(IFilterSetter filter, ISortSetter sort) {
			return Get.Instance<FolderAlbum>(this.FolderPath, filter, sort);
		}
	}
}
