using System;
using System.Linq.Expressions;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;

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
		public Expression<Func<MediaFile, bool>> Condition {
			get;
		}

		/// <summary>
		/// モデル用フィルタリング条件
		/// </summary>
		public Func<IMediaFileModel, bool> ConditionForModel {
			get;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="condition">フィルタリング条件</param>
		/// <param name="conditionForModel">モデル用フィルタリング条件</param>
		public FilterItem(Expression<Func<MediaFile, bool>> condition, Func<IMediaFileModel, bool> conditionForModel) {
			this.Condition = condition;
			this.ConditionForModel = conditionForModel;
		}

		public override string ToString() {
			return $"<[{base.ToString()}] {this.Condition}>";
		}
	}
}
