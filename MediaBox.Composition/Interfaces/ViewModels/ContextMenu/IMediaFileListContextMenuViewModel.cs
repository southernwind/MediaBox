
using System.Collections.Generic;

using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;

namespace SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu {
	public interface IMediaFileListContextMenuViewModel : IViewModelBase {

		void SetTargetFiles(IEnumerable<IMediaFileViewModel> targetFiles);

		void SetTargetAlbum(IAlbumObject albumObject);
	}
}