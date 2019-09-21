using System.Windows.Controls;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class AlbumViewer : ModelBase {
		public string Title {
			get;
		}

		public UserControl Viewer {
			get;
		}

		public AlbumViewer(string title, UserControl viewer) {
			this.Title = title;
			this.Viewer = viewer;
		}
	}
}
