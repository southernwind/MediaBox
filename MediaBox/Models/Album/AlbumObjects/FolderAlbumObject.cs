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
	}
}
