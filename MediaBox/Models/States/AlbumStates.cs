using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// アルバムの状態
	/// </summary>
	public class AlbumStates : NotificationObject {
		public ReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
		} = new ReactiveCollection<IFilterItemCreator>();

		public void LoadDefault() {
			this.FilterItemCreators.Clear();
		}

		public void Load(AlbumStates albumStates) {
			this.FilterItemCreators.Clear();
			this.FilterItemCreators.AddRange(albumStates.FilterItemCreators);
		}

		public void Dispose() {
			this.FilterItemCreators?.Dispose();
		}
	}
}
