using System;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// メディアファイル・タグ中間テーブル
	/// </summary>
	public class MediaFileTag {
		private MediaFile? _mediaFile;
		private Tag? _tag;

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
			get {
				return this._mediaFile ?? throw new InvalidOperationException();
			}
			set {
				this._mediaFile = value;
			}
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
			get {
				return this._tag ?? throw new InvalidOperationException();
			}
			set {
				this._tag = value;
			}
		}
	}
}
