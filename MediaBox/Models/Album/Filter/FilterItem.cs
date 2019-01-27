using System;

using SandBeige.MediaBox.Models.Media;

namespace SandBeige.MediaBox.Models.Album.Filter {
	internal class FilterItem : ModelBase, IFilterItem {
		/// <summary>
		/// フィルタリング条件
		/// </summary>
		public Func<MediaFileModel, bool> Condition {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="condition">フィルタリング条件</param>
		/// <param name="displayName">表示名</param>
		public FilterItem(Func<MediaFileModel, bool> condition) {
			this.Condition = condition;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Condition}>";
		}
	}
}
