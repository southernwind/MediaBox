using System;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Filter {
	/// <summary>
	/// フィルター条件
	/// </summary>
	/// <remarks>
	/// <see cref="FilterItemCreators.IFilterItemCreator"/>から生成する。
	/// </remarks>
	internal class FilterItem : ModelBase, IFilterItem {
		/// <summary>
		/// フィルタリング条件
		/// </summary>
		public Func<IMediaFileModel, bool> Condition {
			get;
		}

		/// <summary>
		/// フィルタリング更新トリガープロパティ名
		/// </summary>
		public string[] Properties {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="condition">フィルタリング条件</param>
		/// <param name="properties">表示名</param>
		public FilterItem(Func<IMediaFileModel, bool> condition, params string[] properties) {
			this.Condition = condition;
			this.Properties = properties;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Condition}>";
		}
	}
}
