using System;

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
		/// <returns>作成されたアルバム</returns>
		public IAlbumModel Create() {
			return Get.Instance<FolderAlbum>(this.FolderPath);
		}
	}
}
