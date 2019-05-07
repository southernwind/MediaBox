using System.Collections.Generic;

using SandBeige.MediaBox.DataBase.Tables.Metadata;

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
		/// ディレクトリパス
		/// </summary>
		public string DirectoryPath {
			get;
			set;
		}

		/// <summary>
		/// ファイル名
		/// </summary>
		public string FilePath {
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
		/// 高度
		/// </summary>
		public double? Altitude {
			get;
			set;
		}

		/// <summary>
		/// ファイルサイズ
		/// </summary>
		public long FileSize {
			get;
			set;
		}

		/// <summary>
		/// 評価
		/// </summary>
		public int Rate {
			get;
			set;
		}

		/// <summary>
		/// 解像度 幅
		/// </summary>
		public int Width {
			get;
			set;
		}

		/// <summary>
		/// 解像度 高さ
		/// </summary>
		public int Height {
			get;
			set;
		}

		/// <summary>
		/// ファイルハッシュ
		/// </summary>
		public string Hash {
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

		/// <summary>
		/// 動画ファイル
		/// </summary>
		public virtual VideoFile VideoFile {
			get;
			set;
		}

		/// <summary>
		/// 画像ファイル
		/// </summary>
		public virtual ImageFile ImageFile {
			get;
			set;
		}

		/// <summary>
		/// Jpegメタデータ
		/// </summary>
		public virtual Jpeg Jpeg {
			get;
			set;
		}

		/// <summary>
		/// Pngメタデータ
		/// </summary>
		public virtual Png Png {
			get;
			set;
		}

		/// <summary>
		/// Bmpメタデータ
		/// </summary>
		public virtual Bmp Bmp {
			get;
			set;
		}
	}
}
