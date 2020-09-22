using System;
using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバム
	/// </summary>
	public class Album {
		private string? _title;
		private AlbumBox? _albumBox;
		private ICollection<AlbumMediaFile>? _albumMediaFiles;
		private ICollection<AlbumScanDirectory>? _albumScanDirectories;

		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			set;
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public string Title {
			get {
				return this._title ?? throw new InvalidOperationException();
			}
			set {
				this._title = value;
			}
		}

		/// <summary>
		/// アルバムボックスID
		/// </summary>
		public int? AlbumBoxId {
			get;
			set;
		}

		/// <summary>
		/// アルバムボックス
		/// </summary>
		public AlbumBox AlbumBox {
			get {
				return this._albumBox ?? throw new InvalidOperationException();
			}
			set {
				this._albumBox = value;
			}
		}

		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public virtual ICollection<AlbumMediaFile> AlbumMediaFiles {
			get {
				return this._albumMediaFiles ?? throw new InvalidOperationException();
			}
			set {
				this._albumMediaFiles = value;
			}
		}

		/// <summary>
		/// 監視対象ディレクトリ
		/// </summary>
		public virtual ICollection<AlbumScanDirectory> AlbumScanDirectories {
			get {
				return this._albumScanDirectories ?? throw new InvalidOperationException();
			}
			set {
				this._albumScanDirectories = value;
			}
		}
	}
}
