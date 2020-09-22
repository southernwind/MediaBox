using System;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバム・メディアファイル中間テーブル
	/// </summary>
	public class AlbumMediaFile {
		private Album? _album;
		private MediaFile? _mediaFile;

		/// <summary>
		/// アルバムId
		/// </summary>
		public int AlbumId {
			get;
			set;
		}

		/// <summary>
		/// アルバム
		/// </summary>
		public Album Album {
			get {
				return this._album ?? throw new InvalidOperationException();
			}

			set {
				this._album = value;
			}
		}

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
