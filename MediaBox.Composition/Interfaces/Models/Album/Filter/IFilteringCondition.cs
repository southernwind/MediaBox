using System;
using System.Collections.Generic;
using System.Reactive;

using LiteDB;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.DataBase.Tables;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter {
	public interface IFilteringCondition : IModelBase {
		IReactiveProperty<string> DisplayName {
			get;
		}
		ReadOnlyReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
		}
		IObservable<Unit> OnUpdateFilteringConditions {
			get;
		}
		IFilterObject RestorableFilterObject {
			get;
		}

		void AddExistsFilter(bool exists);
		void AddFilePathFilter(string text, SearchTypeInclude searchType);
		void AddLocationFilter(bool hasLocation);
		void AddMediaTypeFilter(bool isVideo);
		void AddRateFilter(int rate, SearchTypeComparison searchType);
		void AddResolutionFilter(int? width, int? height, SearchTypeComparison searchType);
		void AddTagFilter(string tagName, SearchTypeInclude searchType);
		void RemoveFilter(IFilterItemCreator filterItemCreator);
		IEnumerable<IMediaFileModel> SetFilterConditions(IEnumerable<IMediaFileModel> files);
		IEnumerable<MediaFile> SetFilterConditions(ILiteQueryable<MediaFile> query);
	}
}