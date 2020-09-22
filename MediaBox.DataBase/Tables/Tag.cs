using System;
using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// タグテーブル
	/// </summary>
	public class Tag {
		private string? _tagName;
		private ICollection<MediaFileTag>? _mediaFileTags;

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
			get {
				return this._tagName ?? throw new InvalidOperationException();
			}
			set {
				this._tagName = value;
			}
		}

		/// <summary>
		/// タグをつけているメディアファイル
		/// </summary>
		public virtual ICollection<MediaFileTag> MediaFileTags {
			get {
				return this._mediaFileTags ?? throw new InvalidOperationException();
			}
			set {
				this._mediaFileTags = value;
			}
		}
	}
}
