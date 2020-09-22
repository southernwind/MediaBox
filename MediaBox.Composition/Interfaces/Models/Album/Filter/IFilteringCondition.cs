using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter {
	public interface IFilteringCondition : IModelBase {
		IReactiveProperty<string> DisplayName {
			get;
		}
		ReadOnlyReactiveCollection<IFilterItemObject> FilterItemObjects {
			get;
		}
		IObservable<Unit> OnUpdateFilteringConditions {
			get;
		}
		IFilterObject FilterObject {
			get;
		}

		void AddExistsFilter(bool exists);
		void AddFilePathFilter(string text, SearchTypeInclude searchType);
		void AddLocationFilter(bool hasLocation);
		void AddMediaTypeFilter(bool isVideo);
		void AddRateFilter(int rate, SearchTypeComparison searchType);
		void AddResolutionFilter(int? width, int? height, SearchTypeComparison searchType);
		void AddTagFilter(string tagName, SearchTypeInclude searchType);
		void RemoveFilter(IFilterItemObject filterItemObject);
		IEnumerable<IMediaFileModel> SetFilterConditions(IEnumerable<IMediaFileModel> files);
		IEnumerable<MediaFile> SetFilterConditions(IQueryable<MediaFile> query);
	}
}