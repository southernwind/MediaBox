using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Media;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort {
	public interface ISortCondition : IModelBase {
		ReactiveCollection<ISortItemCreator> CandidateSortItemCreators {
			get;
		}
		IReactiveProperty<string> DisplayName {
			get;
		}
		IObservable<Unit> OnUpdateSortConditions {
			get;
		}
		ISortObject RestorableSortObject {
			get;
		}
		ReadOnlyReactiveCollection<ISortItemCreator> SortItemCreators {
			get;
		}

		void AddSortItem(ISortItemCreator sortItem);
		IOrderedEnumerable<IMediaFileModel> ApplySort(IEnumerable<IMediaFileModel> items, bool reverse);
		void RemoveSortItem(ISortItemCreator sortItem);
	}
}