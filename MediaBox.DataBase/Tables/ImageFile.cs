using System;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// 画像ファイルテーブル
	/// </summary>
	public class ImageFile {
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

		/// <summary>
		/// 画像の方向
		/// </summary>
		public int? Orientation {
			get;
			set;
		}
	}
}
