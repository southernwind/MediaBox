using System;
using System.Linq.Expressions;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter {
	/// <summary>
	/// フィルターアイテムインターフェイス
	/// </summary>
	public interface IFilterItem : IModelBase {
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
		/// SQLに含めるか否か
		/// SQLに含めない場合、クエリ結果に対してC#上でフィルタリングを行う
		/// </summary>
		public bool IncludeSql {
			get;
		}
	}
}
