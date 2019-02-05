using Livet;

using Reactive.Bindings;

using SandBeige.MediaBox.Library.Extensions;
using SandBeige.MediaBox.Models.Album.Filter.FilterItemCreators;

namespace SandBeige.MediaBox.Models.States {
	/// <summary>
	/// アルバムの状態
	/// </summary>
	public class AlbumStates : NotificationObject {
		/// <summary>
		/// フィルター状態
		/// </summary>
		public ReactiveCollection<IFilterItemCreator> FilterItemCreators {
			get;
		} = new ReactiveCollection<IFilterItemCreator>();

		/// <summary>
		/// ロード
		/// </summary>
		/// <param name="albumStates"></param>
		public void Load(AlbumStates albumStates) {
			this.FilterItemCreators.Clear();
			this.FilterItemCreators.AddRange(albumStates.FilterItemCreators);
		}

		/// <summary>
		/// デフォルトロード
		/// </summary>
		public void LoadDefault() {
			this.FilterItemCreators.Clear();
		}

		/// <summary>
		/// 破棄
		/// </summary>
		public void Dispose() {
			this.FilterItemCreators?.Dispose();
		}
	}
}
