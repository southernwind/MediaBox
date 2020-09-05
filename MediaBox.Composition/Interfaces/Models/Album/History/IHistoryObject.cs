
using SandBeige.MediaBox.Composition.Interfaces.Models.Album.AlbumObjects;

namespace SandBeige.MediaBox.Composition.Interfaces.Models.Album.History {
	public interface IHistoryObject {
		/// <summary>
		/// アルバムオブジェクト
		/// </summary>
		public IAlbumObject AlbumObject {
			get;
		}

		/// <summary>
		/// タイトル
		/// </summary>
		public string Title {
			get;
		}
	}
}
