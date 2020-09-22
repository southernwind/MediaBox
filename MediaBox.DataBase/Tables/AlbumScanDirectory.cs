using System;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバムスキャンディレクトリ
	/// </summary>
	public class AlbumScanDirectory {
		private Album? _album;
		private string? _directory;

		/// <summary>
		/// アルバムID
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
		/// ディレクトリ
		/// </summary>
		public string Directory {
			get {
				return this._directory ?? throw new InvalidOperationException();
			}
			set {
				this._directory = value;
			}
		}
	}
}
