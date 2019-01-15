using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// アルバムの状態
	/// </summary>
	public class AlbumStates : NotificationObject {
		public ReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
		} = new ReactiveCollection<IFilterItemCreator>();

		public void Dispose() {
			this.FilterItemCreators?.Dispose();
		}
	}
}
