using System.Windows.Controls;

using SandBeige.MediaBox.Composition.Interfaces.ViewModels.ContextMenu;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Plugin {
	public interface IAlbumViewerPlugin : IPlugin {
		UserControl CreateViewerControlInstance();

		IAlbumViewerViewModel CreateViewModelInstance(IAlbumViewModel album, IMediaFileListContextMenuViewModel contextMenuViewModel);
	}
}
