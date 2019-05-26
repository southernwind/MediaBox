using System;
using System.Linq;
using System.Reactive;

using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Album.Filter {
	public interface IFilterSetter {
		IQueryable<MediaFile> SetFilterConditions(IQueryable<MediaFile> query);

		/// <summary>
		/// フィルター条件変更通知
		/// </summary>
		IObservable<Unit> OnFilteringConditionChanged {
			get;
		}
	}
}
