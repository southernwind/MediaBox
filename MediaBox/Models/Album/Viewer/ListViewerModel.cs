using Reactive.Bindings;

using SandBeige.MediaBox.Composition.Bases;
using SandBeige.MediaBox.Composition.Enum;
using SandBeige.MediaBox.Composition.Interfaces;
using SandBeige.MediaBox.Composition.Settings;

namespace SandBeige.MediaBox.Models.Album.Viewer {
	public class ListViewerModel : ModelBase {

		/// <summary>
		/// 表示する列
		/// </summary>
		public ReadOnlyReactiveCollection<Col> Columns {
			get;
		}

		public ListViewerModel(IAlbumModel albumModel, ISettings settings) {
			this.Columns = settings.GeneralSettings.EnabledColumns.ToReadOnlyReactiveCollection(x => new Col(x));
		}
	}

	public class Col {
		public AvailableColumns AlternateKey {
			get;
		}

		public Col(AvailableColumns alternateKey) {
			this.AlternateKey = alternateKey;
		}
	}
}
