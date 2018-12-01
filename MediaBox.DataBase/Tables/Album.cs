﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバム
	/// </summary>
	public class Album {
		/// <summary>
		/// アルバムID
		/// </summary>
		public int AlbumId {
			get;
			set;
		}

		/// <summary>
		/// アルバムパス
		/// </summary>
		public string Path {
			get;
			set;
		}

		/// <summary>
		/// アルバムタイトル
		/// </summary>
		public string Title {
			get;
			set;
		}

		/// <summary>
		/// メディアファイルリスト
		/// </summary>
		public virtual ICollection<AlbumMediaFile> AlbumMediaFiles {
			get;
			set;
		}

		/// <summary>
		/// 監視対象ディレクトリ
		/// </summary>
		public virtual ICollection<AlbumDirectory> AlbumDirectories {
			get;
			set;
		}
	}
}