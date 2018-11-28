using System;
using System.Collections.Generic;
using System.Text;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// メディアファイル・タグ中間テーブル
	/// </summary>
	public class MediaFileTag {
		/// <summary>
		/// メディアファイルID
		/// </summary>
		public long MediaFileId {
			get;
			set;
		}

		/// <summary>
		/// メディアファイル
		/// </summary>
		public MediaFile MediaFile {
			get;
			set;
		}

		/// <summary>
		/// タグID
		/// </summary>
		public int TagId {
			get;
			set;
		}

		/// <summary>
		/// タグ
		/// </summary>
		public Tag Tag {
			get;
			set;
		}
	}
}
