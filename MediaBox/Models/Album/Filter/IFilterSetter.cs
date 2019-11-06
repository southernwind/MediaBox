using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Models.Album.Filter {
	public interface IFilterSetter {
		IEnumerable<MediaFile> SetFilterConditions(IQueryable<MediaFile> query);

		IEnumerable<IMediaFileModel> SetFilterConditions(IEnumerable<IMediaFileModel> query);

		/// <summary>
		/// フィルター条件変更通知
		/// </summary>
		IObservable<Unit> OnFilteringConditionChanged {
			get;
		}
	}
}
