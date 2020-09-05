using System;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Models.Album.AlbumObjects {
	/// <summary>
	/// データベース検索アルバム
	/// </summary>
	public class FolderAlbumObject : IAlbumObject {
		/// <summary>
		/// フォルダパス
		/// </summary>
		public string FolderPath {
			get;
			set;
		}


		[Obsolete("for serialize")]
		public FolderAlbumObject() {
			this.FolderPath = null!;
		}

		public FolderAlbumObject(string folderPath) {
			this.FolderPath = folderPath;
		}
	}
}
