namespace SandBeige.MediaBox.DataBase.Tables.Metadata {
	public class VideoMetadataValue {
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
		public VideoFile? MediaFile {
			get;
			set;
		}

		/// <summary>
		/// キー
		/// </summary>
		public string Key {
			get;
			set;
		} = string.Empty;

		/// <summary>
		/// 値
		/// </summary>
		public string Value {
			get;
			set;
		} = string.Empty;
	}
}
