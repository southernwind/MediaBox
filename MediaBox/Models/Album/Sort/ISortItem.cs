using System;
using System.ComponentModel;

namespace SandBeige.MediaBox.Models.Album.Sort {
	internal interface ISortItem : INotifyPropertyChanged {
		/// <summary>
		/// 保存時のキー値
		/// </summary>
		public string Key {
			get;
			set;
		}

		/// <summary>
		/// 表示名
		/// </summary>
		public string DisplayName {
			get;
			set;
		}

		/// <summary>
		/// 有効 / 無効
		/// </summary>
		public bool Enabled {
			get;
			set;
		}

		/// <summary>
		/// ソートの方向
		/// </summary>
		public ListSortDirection Direction {
			get;
			set;
		}

		/// <summary>
		/// 最終変更日時
		/// </summary>
		public DateTime UpdateTime {
			get;
			set;
		}
	}
}
