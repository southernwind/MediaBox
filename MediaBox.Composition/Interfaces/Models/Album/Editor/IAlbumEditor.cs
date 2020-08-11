using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Selector;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.Editor {
	public interface IAlbumEditor : IModelBase {
		IReactiveProperty<int?> AlbumBoxId {
			get;
		}
		IReactiveProperty<string[]> AlbumBoxTitle {
			get;
		}
		IAlbumSelector AlbumSelector {
			get;
		}
		ReactiveCollection<string> MonitoringDirectories {
			get;
		}
		IReactiveProperty<string> Title {
			get;
		}

		void AddDirectory(string path);
		void CreateAlbum();
		void EditAlbum(IEditableAlbumObject albumObject);
		void Load();
		void RemoveDirectory(string path);
		void Save();
		string ToString();
	}
}