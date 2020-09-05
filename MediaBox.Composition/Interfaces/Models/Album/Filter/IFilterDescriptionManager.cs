using System;
using System.Collections.Generic;
using System.Reactive;

using LiteDB;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter {
	public interface IFilterDescriptionManager : IModelBase {
		IReactiveProperty<IFilteringCondition?> CurrentFilteringCondition {
			get;
		}
		ReadOnlyReactiveCollection<IFilteringCondition> FilteringConditions {
			get;
		}
		/// <summary>
		/// フィルター条件変更通知
		/// </summary>
		IObservable<Unit> OnFilteringConditionChanged {
			get;
		}

		/// <summary>
		/// 設定値保存用名前
		/// </summary>
		IReactiveProperty<string> Name {
			get;
		}
		void AddCondition();
		void RemoveCondition(IFilteringCondition filteringCondition);
		IEnumerable<IMediaFileModel> SetFilterConditions(IEnumerable<IMediaFileModel> files);
		IEnumerable<MediaFile> SetFilterConditions(ILiteQueryable<MediaFile> query);
	}
}