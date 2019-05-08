namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	/// <summary>
	/// メタデータテーブル共通部
	/// </summary>
	public abstract class MetadataBase {
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
	}
}
