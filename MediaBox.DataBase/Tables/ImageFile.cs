namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// 画像ファイルテーブル
	/// </summary>
	public class ImageFile {
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
		/// 画像の方向
		/// </summary>
		public int? Orientation {
			get;
			set;
		}
	}
}
