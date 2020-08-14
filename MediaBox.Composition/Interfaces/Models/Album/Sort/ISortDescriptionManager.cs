using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort {
	/// <summary>
	/// ソート適用インターフェイス
	/// </summary>
	public interface ISortDescriptionManager : IModelBase {

		/// <summary>
		/// フィルター条件変更通知
		/// </summary>
		IObservable<Unit> OnSortConditionChanged {
			get;
		}

		/// <summary>
		/// 設定値保存用名前
		/// </summary>
		IReactiveProperty<string> Name {
			get;
		}
		IReactiveProperty<ISortCondition> CurrentSortCondition {
			get;
		}
		IReactiveProperty<ListSortDirection> Direction {
			get;
		}
		ReadOnlyReactiveCollection<ISortCondition> SortConditions {
			get;
		}

		void AddCondition();
		void RemoveCondition(ISortCondition sortCondition);
		/// <summary>
		/// ソート条件適用
		/// </summary>
		/// <param name="array">ソート対象の配列</param>
		/// <returns>ソート済み配列</returns>
		IEnumerable<IMediaFileModel> SetSortConditions(IEnumerable<IMediaFileModel> array);
	}
}
