using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort {
	public interface ISortItem : IModelBase {
		/// <summary>
		/// 保存時のキー値
		/// </summary>
		SortItemKeys Key {
			get;
			set;
		}

		/// <summary>
		/// ソートの方向
		/// </summary>
		ListSortDirection Direction {
			get;
			set;
		}

		/// <summary>
		/// ソート適用
		/// </summary>
		/// <param name="items">ソートを適用するアイテムリスト</param>
		/// <returns>整列されたアイテムリスト</returns>
		IOrderedEnumerable<IMediaFileModel> ApplySort(IEnumerable<IMediaFileModel> items, bool reverse);

		/// <summary>
		/// ソートされたアイテムリストに対して、追加のソート条件適用
		/// </summary>
		/// <param name="items">ソートを適用するアイテムリスト</param>
		/// <returns>整列されたアイテムリスト</returns>
		IOrderedEnumerable<IMediaFileModel> ApplyThenBySort(IOrderedEnumerable<IMediaFileModel> items, bool reverse);
	}
}
