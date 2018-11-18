using System;
using System.Collections.Generic;
using System.Text;

namespace SandBeige.MediaBox.DataBase.Tables {
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
	}
}
