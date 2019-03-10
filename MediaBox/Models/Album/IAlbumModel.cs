using Reactive.Bindings;

namespace SandBeige.MediaBox.Models.Album {
	public interface IAlbumModel {
		/// <summary>
		/// アルバムタイトル
		/// </summary>
		IReactiveProperty<string> Title {
			get;
		}
	}
}
