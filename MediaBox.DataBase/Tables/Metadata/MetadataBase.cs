using System;

namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	/// <summary>
	/// メタデータテーブル共通部
	/// </summary>
	public abstract class MetadataBase {
		private MediaFile? _mediaFile;

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
	}
}
