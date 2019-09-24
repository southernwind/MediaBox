using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	internal class ListViewerModel : ModelBase {

		/// <summary>
		/// 表示する列
		/// </summary>
		public ReadOnlyReactiveCollection<Col> Columns {
			get;
		}

		public ListViewerModel(IAlbumModel albumModel) {
			this.Columns = this.Settings.GeneralSettings.EnabledColumns.ToReadOnlyReactiveCollection(x => new Col(x));
		}
	}

	internal class Col {
		public AvailableColumns AlternateKey {
			get;
		}

		public Col(AvailableColumns alternateKey) {
			this.AlternateKey = alternateKey;
		}
	}
}
