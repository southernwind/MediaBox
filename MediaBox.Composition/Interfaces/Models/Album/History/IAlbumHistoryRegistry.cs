using SandBeige.MediaBox.Composition.Interfaces.Models.Album.Object;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.History {
	/// <summary>
	/// アルバム履歴管理
	/// </summary>
	public interface IAlbumHistoryRegistry : IModelBase {
		/// <summary>
		/// アルバム履歴追加
		/// </summary>
		/// <param name="title">アルバムタイトル</param>
		/// <param name="album">追加対象アルバム</param>
		public void Add(string title, IAlbumObject album) {
		}
	}
}
