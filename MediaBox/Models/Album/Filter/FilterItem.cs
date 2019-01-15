using System;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Filter {
	internal class FilterItem : ModelBase, IFilterItem {
		/// <summary>
		/// フィルタリング条件
		/// </summary>
		public Func<MediaFile, bool> Condition {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="condition">フィルタリング条件</param>
		/// <param name="displayName">表示名</param>
		public FilterItem(Func<MediaFile, bool> condition) {
			this.Condition = condition;
		}
	}
}
