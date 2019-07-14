namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバムスキャンディレクトリ
	/// </summary>
	public class AlbumScanDirectory {
		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			set;
		}

		/// <summary>
		/// アルバム
		/// </summary>
		public Album Album {
			get;
			set;
		}

		/// <summary>
		/// ディレクトリ
		/// </summary>
		public string Directory {
			get;
			set;
		}
	}
}
