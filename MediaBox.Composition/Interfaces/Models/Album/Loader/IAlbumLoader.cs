using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;
using SandBeige.MediaBox.Composition.Interfaces.Models.TaskQueue.Objects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Loader {
	public interface IAlbumLoader : IModelBase {
		IObservable<IEnumerable<IMediaFileModel>> OnAddFile {
			get;
		}
		IObservable<Unit> OnAlbumDefinitionUpdated {
			get;
		}
		IObservable<IEnumerable<IMediaFileModel>> OnDeleteFile {
			get;
		}
		string Title {
			get;
			set;
		}

		int GetBeforeFilteringCount();
		Task<IEnumerable<IMediaFileModel>> LoadMediaFiles(TaskActionState state);
		void SetAlbumObject(IAlbumObject albumObject);
		void SetFilterAndSort(IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter);
	}
}