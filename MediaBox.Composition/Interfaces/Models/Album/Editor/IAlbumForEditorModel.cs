using System.Collections.Generic;
using System.Threading.Tasks;

using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Filter;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Sort;
using SandBeige.MediaBox.Composition.Interfaces.Models.Gesture;
using SandBeige.MediaBox.Composition.Interfaces.Models.Media;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Editor {
	public interface IAlbumForEditorModel : IModelBase {
		IReactiveProperty<int?> AlbumBoxId {
			get;
		}
		IReactiveProperty<int> AlbumId {
			get;
		}
		IReactiveProperty<IMediaFileModel> CurrentMediaFile {
			get;
		}
		IReactiveProperty<IEnumerable<IMediaFileModel>> CurrentMediaFiles {
			get;
		}
		ReactiveCollection<string> Directories {
			get;
		}
		IGestureReceiver GestureReceiver {
			get;
		}
		ReactiveCollection<IMediaFileModel> Items {
			get;
		}
		IReactiveProperty<string> Title {
			get;
		}
		IReadOnlyReactiveProperty<int> ZoomLevel {
			get;
		}

		void AddFiles(IEnumerable<IMediaFileModel> mediaFiles);
		void Create(IEditableAlbumObject editableAlbumObject, IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter);
		Task LoadFromDataBase();
		void ReflectToDataBase();
		void RemoveFiles(IEnumerable<IMediaFileModel> mediaFiles);
		void SetAlbumObject(IEditableAlbumObject editableAlbumObject, IFilterDescriptionManager filterSetter, ISortDescriptionManager sortSetter);
	}
}