using System;
using System.Collections.Generic;

namespace SandBeige.MediaBox.DataBase.Tables {
	/// <summary>
	/// アルバムボックステーブル
	/// </summary>
	public class AlbumBox {
		private string? _name;
		private ICollection<AlbumBox>? _children;
		private ICollection<Album>? _albums;

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
			get {
				return this._name ?? throw new InvalidOperationException();
			}
			set {
				this._name = value;
			}
		}

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
		public ICollection<AlbumBox> Children {
			get {
				return this._children ?? throw new InvalidOperationException();
			}
			set {
				this._children = value;
			}
		}

		/// <summary>
		/// 子アルバム
		/// </summary>
		public ICollection<Album> Albums {
			get {
				return this._albums ?? throw new InvalidOperationException();
			}
			set {
				this._albums = value;
			}
		}
	}
}
