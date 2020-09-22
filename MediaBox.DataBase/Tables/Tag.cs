using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// タグテーブル
	/// </summary>
	public class Tag {
		/// <summary>
		/// タグID
		/// </summary>
		public int TagId {
			get;
			set;
		}

		/// <summary>
		/// タグ名
		/// </summary>
		public string TagName {
			get;
			set;
		}

		/// <summary>
		/// タグをつけているメディアファイル
		/// </summary>
		public virtual ICollection<MediaFileTag> MediaFileTags {
			get;
			set;
		}
	}
}
