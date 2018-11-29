using System;
using System.Collections.Generic;
using System.Text;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバムディレクトリ
	/// </summary>
	public class AlbumDirectory {
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
			get;
			set;
		}

		/// <summary>
		/// ディレクトリ
		/// </summary>
		public string Directory {
			get;
			set;
		}
	}
}
