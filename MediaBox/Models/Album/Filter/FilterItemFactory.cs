
using System;

using Prism.Ioc;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemObjects;

namespace SandBeige.MediaBox.Models.Album.Filter {
	public class FilterItemFactory : IFilterItemFactory {
		private readonly IContainerProvider _containerProvider;
		public FilterItemFactory(IContainerProvider containerProvider) {
			this._containerProvider = containerProvider;
		}

		public IFilterItem Create<T>(T filterItemObject) where T : IFilterItemObject {
			switch (filterItemObject) {
				case ExistsFilterItemObject ef:
					var efIc = this._containerProvider.Resolve<ExistsFilterItemCreator>();
					return efIc.Create(ef);
				case FilePathFilterItemObject fpf:
					var fpfIc = this._containerProvider.Resolve<FilePathFilterItemCreator>();
					return fpfIc.Create(fpf);
				case LocationFilterItemObject lf:
					var lfIc = this._containerProvider.Resolve<LocationFilterItemCreator>();
					return lfIc.Create(lf);
				case MediaTypeFilterItemObject mtf:
					var mtfIc = this._containerProvider.Resolve<MediaTypeFilterItemCreator>();
					return mtfIc.Create(mtf);
				case RateFilterItemObject rf:
					var rfIc = this._containerProvider.Resolve<RateFilterItemCreator>();
					return rfIc.Create(rf);
				case ResolutionFilterItemObject resolutionFilter:
					var resolutionFilterIc = this._containerProvider.Resolve<ResolutionFilterItemCreator>();
					return resolutionFilterIc.Create(resolutionFilter);
				case TagFilterItemObject tf:
					var tfIc = this._containerProvider.Resolve<TagFilterItemCreator>();
					return tfIc.Create(tf);
				default:
					throw new ArgumentException();
			}

		}
	}
}
