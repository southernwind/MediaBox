using System;
using System.Collections.Generic;
using System.Reactive;

using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Sort {
	/// <summary>
	/// ソート適用インターフェイス
	/// </summary>
	public interface ISortSetter {
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
	}
}
