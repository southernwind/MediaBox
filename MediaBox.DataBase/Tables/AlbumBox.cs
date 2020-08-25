using System;
using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバムボックステーブル
	/// </summary>
	public class AlbumBox {
		/// <summary>
		/// ID
		/// </summary>
		public int AlbumBoxId {
			get;
			set;
		}

		/// <summary>
		/// アルバムボックス名
		/// </summary>
		public string Name {
			get;
			set;
		} = string.Empty;

		/// <summary>
		/// 親アルバムボックスID
		/// </summary>
		public int? ParentAlbumBoxId {
			get;
			set;
		}

		/// <summary>
		/// 親アルバムボックス
		/// </summary>
		public AlbumBox? Parent {
			get;
			set;
		}

		/// <summary>
		/// 子アルバムボックス
		/// </summary>
		public ICollection<AlbumBox>? Children {
			get;
			set;
		} = Array.Empty<AlbumBox>();

		/// <summary>
		/// 子アルバム
		/// </summary>
		public ICollection<Album>? Albums {
			get;
			set;
		} = Array.Empty<Album>();
	}
}
