using System;
using System.Collections.Generic;
using System.Reactive;

using Reactive.Bindings;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort {
	/// <summary>
	/// ソート適用インターフェイス
	/// </summary>
	public interface ISortSetter : IModelBase {
		/// <summary>
		/// ソート条件適用
		/// </summary>
		/// <param name="array">ソート対象の配列</param>
		/// <returns>ソート済み配列</returns>
		IEnumerable<IMediaFileModel> SetSortConditions(IEnumerable<IMediaFileModel> array);

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
	}
}
