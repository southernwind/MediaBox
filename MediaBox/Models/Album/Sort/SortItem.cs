using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソート条件
	/// </summary>
	public class SortItem<TKey> : ModelBase, ISortItem {
		/// <summary>
		/// 保存時のキー値
		/// </summary>
		public SortItemKeys Key {
			get {
				return this.GetValue<SortItemKeys>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// ソートの方向
		/// </summary>
		public ListSortDirection Direction {
			get {
				return this.GetValue<ListSortDirection>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// ソートキー
		/// </summary>
		public Func<IMediaFileModel, TKey> KeySelector {
			get {
				return this.GetValue<Func<IMediaFileModel, TKey>>();
			}
			set {
				this.SetValue(value);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="key">保存時のキー</param>
		/// <param name="direction">ソート方向</param>
		public SortItem(SortItemKeys key, Func<IMediaFileModel, TKey> keySelector, ListSortDirection direction = ListSortDirection.Ascending) {
			this.Key = key;
			this.KeySelector = keySelector;
			this.Direction = direction;
		}

		/// <summary>
		/// ソート適用
		/// </summary>
		/// <param name="items">ソートを適用するアイテムリスト</param>
		/// <param name="reverse">ソート方向の反転を行うか否か true:反転する false:反転しない</param>
		/// <returns>整列されたアイテムリスト</returns>
		public IOrderedEnumerable<IMediaFileModel> ApplySort(IEnumerable<IMediaFileModel> items, bool reverse) {
			if (this.Direction == ListSortDirection.Ascending ^ reverse) {
				return items.OrderBy(this.KeySelector);
			} else {
				return items.OrderByDescending(this.KeySelector);
			}
		}

		/// <summary>
		/// ソートされたアイテムリストに対して、追加のソート条件適用
		/// </summary>
		/// <param name="items">ソートを適用するアイテムリスト</param>
		/// <param name="reverse">ソート方向の反転を行うか否か true:反転する false:反転しない</param>
		/// <returns>整列されたアイテムリスト</returns>
		public IOrderedEnumerable<IMediaFileModel> ApplyThenBySort(IOrderedEnumerable<IMediaFileModel> items, bool reverse) {
			if (this.Direction == ListSortDirection.Ascending ^ reverse) {
				return items.ThenBy(this.KeySelector);
			} else {
				return items.ThenByDescending(this.KeySelector);
			}
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Key}>";
		}
	}
}
