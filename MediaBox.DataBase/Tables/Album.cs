using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバム
	/// </summary>
	public class Album {
		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			set;
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public int? AlbumBoxId {
			get;
			set;
		}

		/// <summary>
		/// アルバムボックス
		/// </summary>
		public AlbumBox AlbumBox {
			get;
			set;
		}

		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public virtual ICollection<AlbumMediaFile> AlbumMediaFiles {
			get;
			set;
		}

		/// <summary>
		/// 監視対象ディレクトリ
		/// </summary>
		public virtual ICollection<AlbumScanDirectory> AlbumScanDirectories {
			get;
			set;
		}
	}
}
