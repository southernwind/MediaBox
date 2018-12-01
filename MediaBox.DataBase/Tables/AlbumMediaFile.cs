﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバム・メディアファイル中間テーブル
	/// </summary>
	public class AlbumMediaFile {
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
			get;
			set;
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
			get;
			set;
		}
	}
}