using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// メディアファイルテーブル
	/// </summary>
	public class MediaFile {
		/// <summary>
		/// メディアファイルID
		/// </summary>
		public long MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// ファイルが格納されているディレクトリパス
		/// </summary>
		public string DirectoryPath {
			get;
			set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FileName {
			get;
			set;
		}

		/// <summary>
		/// サムネイルファイル名
		/// </summary>
		public string ThumbnailFileName {
			get;
			set;
		}

		/// <summary>
		/// 緯度
		/// </summary>
		public double? Latitude {
			get;
			set;
		}

		/// <summary>
		/// 経度
		/// </summary>
		public double? Longitude {
			get;
			set;
		}

		/// <summary>
		/// 画像の方向
		/// </summary>
		public int? Orientation {
			get;
			set;
		}

		/// <summary>
		/// このメディアファイルを含んでいるアルバムリスト
		/// </summary>
		public virtual ICollection<AlbumMediaFile> AlbumMediaFiles {
			get;
			set;
		}

		/// <summary>
		/// タグリスト
		/// </summary>
		public virtual ICollection<MediaFileTag> MediaFileTags {
			get;
			set;
		}
	}
}
